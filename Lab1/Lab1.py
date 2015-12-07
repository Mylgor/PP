import threading
import string

numbOfSymbol = 500
numbOfString = 100

def PrintLetter(letter):
    for _ in range(numbOfSymbol):
        print(letter, end="")
    print()

def GetThread(_threads):
    for index in range(len(_threads)):
        if not(_threads[index].isAlive()):
            return index
    return -1

def none(countFlows):
    threads = [threading.Thread()] * countFlows
    for letter in string.ascii_lowercase:
        for _ in range(numbOfString):
            ind = GetThread(threads)
            while ind == -1:
                ind = GetThread(threads)
            t = threading.Thread(target=PrintLetter, args=[letter])
            t.start()
            threads[ind] = t

def look(countFlows):
    lock = threading.Lock()
    threads = [threading.Thread()] * countFlows
    for letter in string.ascii_lowercase:
        for _ in range(numbOfString):
            ind = GetThread(threads)
            while ind == -1:
                ind = GetThread(threads)
            t = threading.Thread(target=PrintLetter, args=[letter])
            t.start()
            threads[ind] = t

def semaphore(countFlows):
    semaphore = threading.BoundedSemaphore(1)
    threads = [threading.Thread()] * countFlows
    for letter in string.ascii_lowercase:
        for _ in range(numbOfString):
            ind = GetThread(threads)
            while ind == -1:
                ind = GetThread(threads)
            t = threading.Thread(target=PrintLetter, args=[letter])
            t.start()
            threads[ind] = t

def event(countFlows):
    event = [threading.Event()] * countFlows
    threads = [threading.Thread()] * countFlows
    event[-1].set()
    for letter in string.ascii_lowercase:
        for _ in range(numbOfString):
            ind = GetThread(threads)
            while ind == -1:
                ind = GetThread(threads)
            event[ind - 1].wait()
            event[ind - 1].clear()
            t = threading.Thread(target=PrintLetter, args=[letter])
            event[ind].set()
            t.start()
            threads[ind] = t

try:
    answer = int (input("Choose\n1 - none\n2 - look\n3 - semaphore\n4 - event\nAnswer: "))
    countFlows = int(input("Numb of flows: "))
except ValueError:
    print("error")
    exit(1)

if answer == 1:
    none(countFlows)
elif answer == 2:
    look(countFlows)
elif answer == 3:
    semaphore(countFlows)
elif answer == 4:
    event(countFlows)