
import random
def init(id: str, seed: int) -> list[str]:
    random.seed(seed)
    return [random.randint(1,1000) for _ in range(9)] + [0]

def last(id: str, seed: int) -> list[str]:
    return [max(init(id, seed))]
