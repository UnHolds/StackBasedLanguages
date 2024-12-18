from flask import Flask, request, jsonify
import hashlib
import os
import subprocess


app = Flask(__name__)

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

    # execute file
    result = subprocess.run(['gforth', 'safeforth.fs', path, '-e', 'bye'], stdout=subprocess.PIPE)

    # remove file
    try:
        os.remove(path)
    except:
        pass

    print(result.stdout)
    res = {
        'result': 'fail',
        'reason': ''
    }
    if result.returncode != 0:
        res['reason'] = 'gforth exited with error'

    return jsonify(res)

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=8000, debug=True)
