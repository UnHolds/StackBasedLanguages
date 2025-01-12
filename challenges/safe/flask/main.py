from flask import Flask, request, jsonify
import hashlib
import os
import subprocess
import logging
import sys
import values
import random

app = Flask(__name__)
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("gunicorn.error")

@app.route("/challenge", methods = ['POST'])
def challenge():
    content = request.json
    if 'code' not in content or 'id' not in content:
        return 'bad request!', 400

    if type(content['code']) is not str or type(content['id']) is not str:
        return f'bad request!', 400

    hash = hashlib.sha256()
    hash.update(content['id'].encode('UTF-8'))
    path = f'{hash.digest().hex()}_ch.fs'
    # write the file
    with open(path, 'w') as f:
        f.write(content['code'])
        print(f"writing: {path}")

    safe_command_args = []
    for file in filter(lambda f: not f.startswith('.'), os.listdir("./safe_files")):
        safe_command_args.append("-e")
        safe_command_args.append(f's" safe_files/{file}"')


    nonce = str(abs(int.from_bytes(os.urandom(3), sys.byteorder)) + 10000)
    seed = int.from_bytes(os.urandom(3), sys.byteorder)

    init_valus = []
    for val in values.init(content["id"], seed):
        init_valus.append("-e")
        init_valus.append(str(val))

    last_valus = []
    for val in values.last(content["id"], seed):
        last_valus.append("-e")
        last_valus.append(str(val))

    # execute file
    result = subprocess.run(['gforth', "setup.fs"] + init_valus + ["-e", "0"] + safe_command_args + ['safeforth.fs', path, "-e", nonce, "-e", "."] + last_valus + [ "checker.fs", '-e', 'bye'], stdout=subprocess.PIPE, stderr=subprocess.PIPE)

    # remove file
    try:
        os.remove(path)
    except:
        pass

    res = {
        'result': 'fail',
        'reason': ''
    }
    error_output = result.stderr.decode("utf-8").strip()
    std_output = result.stdout.decode("utf-8").strip()

    logger.info(f"Gforth result: {std_output}")
    logger.info(f"Gforth error: {error_output}")

    #error handling
    if result.returncode != 0:
        error_msg = 'gforth exited with error'
        if error_output.startswith("in file included from") and "checker.fs" not in error_output:
            error_msg = "error: ".join(error_output.split("error: ")[1:])
        res['reason'] = error_msg
    else:
        checker_res = std_output.split(nonce)[-1].strip()
        if checker_res == "0":
            # win condition
            res = {
                'result': 'success',
                'reason': 'You did it :)'
            }
        else:
            res = {
                'result': 'fail',
                'reason': 'The solution is not correct' if checker_res == "1" else checker_res
            }

    return jsonify(res)

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=8000, debug=True)
