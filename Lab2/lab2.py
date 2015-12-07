# -*- coding: utf-8 -*-
from time import time
from random import random
from multiprocessing import Queue, Process
from math import sqrt

def get_number_of_point(que, iterat):
    count = 0
    for i in range(iterat):
        x = random()
        y = random()
        if sqrt((x * x) + (y * y)) <= 1:
            count += 1
    que.put(count)

if __name__ == '__main__':
    try:
        count_iter = int(input("count iter: "))
        count_proc = int(input("count proc: "))
    except:
        print("error")
        exit(1)

    timer = time()

    numberIter = [count_iter // count_proc] * count_proc
    for i in range(count_iter % count_proc):
        numberIter[i] += 1

    queue = [Queue()] * count_proc
    mass = []
    point_in_cir = 0

    for i in range(count_proc):
        mass.append(Process(target=get_number_of_point, args=(queue[i], numberIter[i])))
        mass[i].start()

    for i in range(count_proc):
        point_in_cir += queue[i].get()
        mass[i].join()

    pi = point_in_cir / count_iter * 4

    endTimer = time()
    print ("Pi: ", pi)
    print("time: ", endTimer - timer)
