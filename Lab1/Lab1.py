# -*- coding: utf-8 -*-
# Найти битые сслыки на сайте и вывести сначала хорошие ссылки, затем битые ссылки в файл
# Выполнил: Долгушев Евгений ПС-31

import sys
import urllib, socket
import re
from urlparse import urlparse
from urlparse import urljoin


def CheckURLScheme(url):
    scheme = urlparse(url).scheme
    if (len(scheme) > 0) and (scheme not in ['http', 'https']):
        return False


    return url[0] != '#' and len(url) > 2


def CompareStack(new):
    global originsUrls

    for url in new:
        if (CheckURLScheme(url)):
            parsed = urlparse(url)
            if parsed.netloc == '' and parsed.scheme == '':
                url = urljoin(mainUrl, url)
            if (url not in originsUrls):
                    originsUrls.append(url)


def GetDomenName(netloc):
    first = netloc[:netloc.find('.')]
    domen = netloc[netloc.find('.') + 1: len(netloc)]
    if (domen.find('.') == -1):
        return netloc
    else:
        return domen


def CheckNetloc(url):
    newUrlNetLoc = urlparse(url).netloc
    mainUrlNetLoc = urlparse(mainUrl).netloc

    return newUrlNetLoc == mainUrlNetLoc


def CheckPages(url):
    global failedPages
    global accessedPages
    global originsUrls
    global currentUrl
    global mainUrl

    #pause = input('Pause : ')
    #<a href="(.+?)".+?>

    if (CheckURLScheme(url) == False):
        newUrl = urljoin(originsUrls[0], url)
        CheckPages(newUrl)
    else:
        parsed = urlparse(url)
        if parsed.netloc == '' and parsed.scheme == '':
            url = urljoin(mainUrl, url)

        #newUrlNetLoc = urlparse(url).netloc
        #urlNetLoc = urlparse(mainUrl).netloc

        html = ''
        if (CheckNetloc(url)):
            try:
                html = urllib.urlopen(url)
            except socket.error, e:
                print "Ping error: ", e

            data = html.read()
            urls = []
            urls.extend(re.findall('<a href="(.+?)".+?>', data, re.DOTALL))
            CompareStack(urls)

            currentUrl = currentUrl + 1
            #print currentUrl,
            #print url,
            #print originsUrls,
            #print
        else:
            currentUrl = currentUrl + 1


def WriteToFile(output, url, code):
    output.write(url)
    output.write(' - ')
    output.write(str(code))
    output.write('\n')


def FillReport(url, code, failedPages, accessedPages):
    output = open('report.txt', 'w')
    WriteToFile(output, url, code)

    for key in accessedPages:
        WriteToFile(output, key, accessedPages[key])

    output.write('\n')

    for key in failedPages:
        WriteToFile(output, key, failedPages[key])

    output.close()


def CheckUrls(originsUrls):
    global failedPages
    global accessedPages

    for url in originsUrls:
        try:
            html = urllib.urlopen(url)
            code = html.getcode()
            if (code in [200, 301]):
                accessedPages[url] = code
            else:
                failedPages[url] = code
        except socket.error, e:
            print "Ping error: ", e


#начало исполняемого кода
failedPages = {}
accessedPages = {}
originsUrls = []
mainUrl = ''
currentUrl = 0

if __name__ == "__main__":
    if len (sys.argv) == 2:
        mainUrl = sys.argv[1]
        if (CheckURLScheme(mainUrl) == True):
            try:
                code = urllib.urlopen(mainUrl).getcode()
                if (code in [200, 301]):
                    originsUrls.append(mainUrl)

                    while (currentUrl < len(originsUrls)):
                        CheckPages(originsUrls[currentUrl])

                    #print originsUrls,
                    #print
                    CheckUrls(originsUrls)
                    FillReport(mainUrl, code, failedPages, accessedPages)
                else:
                    output = open('report.txt', 'w')
                    WriteToFile(output, mainUrl, code)
            except socket.error, e:
                print "Ping error: ", e

    else:
        sys.exit("Error. Wrong number of arguments.", 1)