
import random
def init(uuid: str, seed: int) -> list[str]:
    random.seed(seed)
    return [random.randint(1,1000), random.randint(1,1000)] #example ['s"test"', 5]

def last(uuid: str, seed: int) -> list[str]:
    return init(uuid, seed)
