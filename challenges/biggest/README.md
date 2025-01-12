
## Solution

```json
{
    "id": "alex",
    "code": ": g 10 1 u+do 2dup > if drop else swap drop endif loop ; g"
}
```

```sh
curl -X POST 127.0.0.1:8081/challenge -H 'Content-Type: application/json' -d '{"id": "alex", "code": ": g 10 1 u+do 2dup > if drop else swap drop endif loop ; g"}'
```
