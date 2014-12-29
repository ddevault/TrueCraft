#!/usr/bin/env python3
# Usage:
# Regenerate specific block: ./generate-blocks [id]
# Regenerate all blocks: ./generate-blocks all
# Note that this isn't perfect and manual tweaking will be required later on
import os
import sys
import json
import requests
from jinja2 import Template, Environment, FileSystemLoader, Markup

env = Environment(loader=FileSystemLoader("templates"))
template = env.get_template("Block.cs")

r = requests.get("http://b.wiki.vg/json/b1.7")

def get_name(block):
    name = None
    if 'name' in block:
        name = block['name']
    if 'display_name' in block:
        name = block['display_name']
    return name

def get_csname(name):
    if name.startswith('Block of '):
        name = name[len('Block of '):] + "Block"
    return name.replace(' ', '').replace("'", "")

def gen_block(block):
    name = get_name(block)
    if not name:
        sys.stderr.write("Skipping block 0x{:02X}\n".format(block['id']))
        return None
    sys.stderr.write("Generating block " + name + "\n")
    csname = get_csname(name)
    params = {
        'id': "0x{:02X}".format(block['id']),
        'hardness': block['hardness'],
        'display_name': name,
        'csname': csname
    }
    if 'texture' in block:
        params['tex_x'] = block['texture']['x']
        params['tex_y'] = block['texture']['y']
    return template.render(**params)

def do_block(block):
    code = gen_block(block)
    if code == None:
        return
    with open(os.path.join("../TrueCraft.Core/Logic/Blocks/" + \
        get_csname(get_name(block)) + "Block.cs"), 'w') as f:
        f.write(code)

target = sys.argv[1]
blocks = r.json()[0]['blocks']['block']
if target == 'all':
    for key in blocks:
        do_block(blocks[key])
else:
    do_block(blocks[target])
