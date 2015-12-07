# -*- coding: utf-8 -*-
import threading
import string

numb_of_symbol = 500
numb_of_string = 100

def print_letter(letter):
    for _ in range(numb_of_symbol):
        print(letter, end = "")
    print()

def get_thread(threads):
    for index in range(len(threads)):
        if not(threads[index].isAlive()):
            return index
    return -1

def none(count_flows):
    threads = [threading.Thread()] * count_flows
    for letter in string.ascii_lowercase:
        for _ in range(numb_of_string):
            ind = get_thread(threads)
            while ind == -1:
                ind = get_thread(threads)
            t = threading.Thread(target=print_letter, args=[letter])
            t.start()
            threads[ind] = t

def look(count_flows):
    lock = threading.Lock()
    threads = [threading.Thread()] * count_flows
    for letter in string.ascii_lowercase:
        for _ in range(numb_of_string):
            ind = get_thread(threads)
            while ind == -1:
                ind = get_thread(threads)
            t = threading.Thread(target=print_letter, args=[letter])
            t.start()
            threads[ind] = t

def semaphore(count_flows):
    semaphore = threading.BoundedSemaphore(1)
    threads = [threading.Thread()] * count_flows
    for letter in string.ascii_lowercase:
        for _ in range(numb_of_string):
            ind = get_thread(threads)
            while ind == -1:
                ind = get_thread(threads)
            t = threading.Thread(target=print_letter, args=[letter])
            t.start()
            threads[ind] = t

def event(countFlows):
    event = [threading.Event()] * countFlows
    threads = [threading.Thread()] * countFlows
    event[-1].set()
    for letter in string.ascii_lowercase:
        for _ in range(numb_of_string):
            ind = get_thread(threads)
            while ind == -1:
                ind = get_thread(threads)
            event[ind - 1].wait()
            event[ind - 1].clear()
            t = threading.Thread(target=print_letter, args=[letter])
            event[ind].set()
            t.start()
            threads[ind] = t

if __name__ == '__main__':
    try:
        answer = int(input("Choose\n1 - none\n2 - look\n3 - semaphore\n4 - event\nAnswer: "))
        count_flows = int(input("Numb of flows: "))
    except ValueError:
        print("error")
        exit(1)

    if answer == 1:
        none(count_flows)
    elif answer == 2:
        look(count_flows)
    elif answer == 3:
        semaphore(count_flows)
    elif answer == 4:
        event(count_flows)