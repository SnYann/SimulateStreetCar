# encoding='utf-8'
import matplotlib.pyplot as plt
import numpy as np
import tkinter.filedialog

path=tkinter.filedialog.askopenfilename(filetypes=[("文本格式","")])
# path='100000.txt'
fr=open(path,encoding='utf-8')
lines=[inst for inst in fr.readlines()]
steps=[]
people=[]
allTime=[]
deltaTime=[]
for l in lines:
    if "\t" in l:
        line=l.strip().split("\t")
        steps.append(int(line[0].split(" ")[1])*6)
        people.append(30000-int(line[1].split(" ")[1]))
        allTime.append(float(line[2].split(" ")[1])/60)
        deltaTime.append(float(line[3].split(" ")[1]))
# plt.figure(path)
# plt.plot(steps, people, allTime)
# plt.show()
fig = plt.figure(path)
ax1 = fig.add_subplot(111)
ax1.plot(steps,people)
ax1.set_ylabel("people")

# ax2 = ax1.twinx()  # this is the important function
# ax2.plot(steps, allTime, 'r')
# ax2.set_ylabel('seconds')
# ax2.set_xlabel('steps')

plt.show()

# plt.plot(deltaTime)
# plt.show()