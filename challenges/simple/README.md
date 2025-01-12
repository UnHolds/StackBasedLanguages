
# Simple

## Description

Add two numbers and leave the result on the stack

## Input Stack

Two numbers
```
<number>
<number>
```

## Output Stack

Addition result
```
<number>
```

## Keywords
```
=
+
```

## Solution

```json
{
    "id": "alex",
    "code": "+"
}
```

```sh
curl -X POST 127.0.0.1:8080/challenge -H 'Content-Type: application/json' -d '{"id": "alex", "code": "+"}'
```
