
import random
def init(id: str, seed: int) -> list[str]:
    random.seed(seed)
    return [random.randint(1,1000), random.randint(1,1000)] #example ['s"test"', 5]

def last(id: str, seed: int) -> list[str]:
    return init(id, seed)
