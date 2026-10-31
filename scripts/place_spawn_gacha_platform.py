"""One-time vanilla-world edit for the three spawn-area gacha stations.

Only vanilla wood blocks and platforms are written to the .wld. The mod places
its machine tiles at load time, leaving the distributed .wld vanilla-readable.
"""

import os
import struct
import uuid
from pathlib import Path


WORLD = Path(__file__).resolve().parents[1] / "Maps" / "Madake_Adventure.wld"
EXPECTED_GUID = uuid.UUID("4f238fb5-44c5-428c-932d-d394743e1a08")
SPAWN = (4201, 451)
WIDTH, HEIGHT = 8400, 2400


def read_string(data, offset):
    length = shift = 0
    while True:
        value = data[offset]
        offset += 1
        length |= (value & 127) << shift
        if value < 128:
            break
        shift += 7
    return data[offset:offset + length].decode("utf-8"), offset + length


def read_record(data, offset, framed):
    start = offset
    first = data[offset]
    offset += 1
    second = third = 0
    if first & 1:
        second = data[offset]
        offset += 1
    if second & 1:
        third = data[offset]
        offset += 1
    if third & 1:
        offset += 1

    if first & 2:
        tile_type = data[offset]
        offset += 1
        if first & 32:
            tile_type |= data[offset] << 8
            offset += 1
        if tile_type >= len(framed) or framed[tile_type]:
            offset += 4
        if third & 8:
            offset += 1
    if first & 4:
        offset += 1 + bool(third & 16)
    if first & 24:
        offset += 1
    if third & 64:
        offset += 1

    raw = bytes([first & 63]) + data[start + 1:offset]
    run_length = 1
    if first >> 6 == 1:
        run_length += data[offset]
        offset += 1
    elif first >> 6 >= 2:
        run_length += struct.unpack_from("<h", data, offset)[0]
        offset += 2
    return raw, run_length, offset


def encode_column(tiles):
    output = bytearray()
    index = 0
    while index < len(tiles):
        raw = tiles[index]
        end = index + 1
        while end < len(tiles) and tiles[end] == raw and end - index < 32768:
            end += 1
        repeats = end - index - 1
        output.extend(bytes([raw[0] | (128 if repeats > 255 else 64 if repeats else 0)]) + raw[1:])
        if repeats:
            output.extend(struct.pack("<h", repeats) if repeats > 255 else bytes([repeats]))
        index = end
    return bytes(output)


def main():
    original = WORLD.read_bytes()
    version = struct.unpack_from("<i", original)[0]
    assert version == 279, "Unexpected world version"
    count = struct.unpack_from("<h", original, 24)[0]
    pointers = list(struct.unpack_from("<" + "i" * count, original, 26))
    frame_offset = 26 + 4 * count
    frame_count = struct.unpack_from("<h", original, frame_offset)[0]
    frame_offset += 2
    framed = [bool(original[frame_offset + i // 8] & (1 << (i % 8))) for i in range(frame_count)]
    assert not framed[30] and framed[19]

    title, offset = read_string(original, pointers[0])
    _, offset = read_string(original, offset)
    offset += 8
    world_guid = uuid.UUID(bytes_le=original[offset:offset + 16])
    offset += 16
    world_id, _, _, _, _, height, width = struct.unpack_from("<7i", original, offset)
    offset += 28
    spawn = struct.unpack_from("<ii", original, offset + 89)
    assert world_guid == EXPECTED_GUID and (width, height) == (WIDTH, HEIGHT) and spawn == SPAWN

    edited_x = set(range(4191, 4211))
    columns = []
    tiles = {}
    offset = pointers[1]
    for x in range(width):
        start = offset
        y = 0
        column = [] if x in edited_x else None
        while y < height:
            raw, length, offset = read_record(original, offset, framed)
            if column is not None:
                column.extend([raw] * length)
            y += length
        assert y == height
        columns.append(original[start:offset])
        if column is not None:
            tiles[x] = column
    assert offset == pointers[2]

    empty = b"\x00"
    wood = b"\x02\x1e"  # Vanilla Wood Block (TileID 30)
    platform = b"\x02\x13\x00\x00\x00\x00"  # Vanilla Wood Platform (TileID 19)

    for x in edited_x:
        for y in range(432, 450):
            tiles[x][y] = empty
        tiles[x][450] = wood
    for x in range(4196, 4207):
        tiles[x][438] = wood
    for x in range(4198, 4201):
        tiles[x][445] = platform
    for x in range(4202, 4205):
        tiles[x][442] = platform

    # The three 4x5 machine footprints must remain empty, with solid support.
    for center_x, bottom_y in ((4195, 449), (4207, 449), (4201, 437)):
        left, top = center_x - 2, bottom_y - 4
        for x in range(left, left + 4):
            assert all(tiles[x][y] == empty for y in range(top, top + 5))
            assert tiles[x][bottom_y + 1] == wood

    for x in edited_x:
        columns[x] = encode_column(tiles[x])

    sections = [original[pointers[i]:pointers[i + 1]] for i in range(count - 1)]
    sections.append(original[pointers[-1]:])
    sections[1] = b"".join(columns)
    output = bytearray(original[:pointers[0]])
    new_pointers = []
    for section in sections:
        new_pointers.append(len(output))
        output.extend(section)
    struct.pack_into("<" + "i" * count, output, 26, *new_pointers)

    # Verify the entire tile stream and every untouched section before replacing.
    offset = new_pointers[1]
    for x in range(width):
        start = offset
        y = 0
        decoded = [] if x in edited_x else None
        while y < height:
            raw, length, offset = read_record(output, offset, framed)
            if decoded is not None:
                decoded.extend([raw] * length)
            y += length
        assert y == height
        if decoded is not None:
            assert decoded == tiles[x]
        else:
            assert output[start:offset] == columns[x]
    assert offset == new_pointers[2]
    for i in range(count):
        if i != 1:
            assert sections[i] == original[pointers[i]:pointers[i + 1] if i + 1 < count else len(original)]

    temporary = WORLD.with_suffix(".wld.tmp")
    temporary.write_bytes(output)
    os.replace(temporary, WORLD)
    print(f"Updated {WORLD.name}: {title}, world ID {world_id}, spawn {spawn}, {len(output)} bytes")


if __name__ == "__main__":
    main()
