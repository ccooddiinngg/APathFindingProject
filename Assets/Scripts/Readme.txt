//A* Path Finding Algorithm

OPEN: the set of nodes to be evaluated
CLOSED: the set of nodes already evaluated

add the satrt node to OPEN

loop
    current = node in OPEN with the lowest FCost
    remove current form OPEN
    add current to CLOSED

    if current is the target node // path found!
    return

    foreach neighbour of the current node
        if neighbour is not traversable or neighbour is in CLOSED
        skip to next neighbour

        if new path to neighbour is shorter OR neighbour is not in OPEN
            set fCost of neighbour
            set parent of neighbour to current
            if neighbour is not in OPEN
                add neighbour to OPEN 

