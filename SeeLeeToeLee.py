import sys
import random
from functools import reduce
from copy import copy

smallChars="ァィゥェォヵヶッャュョヮ";
toPlainChar=[
	["ァ",  "ア"],["ィ",  "イ"],["ゥヴ","ウ"],["ェ",  "エ"],["ォ", "オ"],
	["ヵガ","カ"],["ギ",  "キ"],["グ",  "ク"],["ヶゲ","ケ"],["ゴ", "コ"],
	["ザ",  "サ"],["ジ",  "シ"],["ズ",  "ス"],["ゼ","  セ"],["ゾ", "ソ"],
	["ダ",  "タ"],["ヂ",  "チ"],["ッヅ","ツ"],["デ",  "テ"],["ド", "ト"],
	["バパ","ハ"],["ビピ","ヒ"],["ブプ","フ"],["ベペ","ヘ"],["ボポ","ホ"],
	["ャ",  "ヤ"],["ュ",  "ユ"],["ョ",  "ヨ"],["ヮ",  "ワ"],
	["2", "ツ"],["Z", "ゼット"],["♂","オス"],["♀","メス"]
]

def check(lastWord,nextWord):
	lastWord=lastWord.replace("ー","");
	isLastSmall=lastWord[-1] in smallChars

	for v in toPlainChar:
		lastWord=reduce(lambda s,sep: s.replace(sep,v[1]),v[0],lastWord)
		nextWord=reduce(lambda s,sep: s.replace(sep,v[1]),v[0],nextWord)

	if(lastWord[-1]==nextWord[0] or
		isLastSmall and lastWord[-2:]==nextWord[:2]):
			return True
	return False

def main(args):
	dataPath=args[0] if 0<len(args) else "./data.csv";
	dataArr=""
	try:
		with open(dataPath,"r",encoding="utf-8") as f:
			dataArr=(f.read()
				.strip()
				.splitlines())
	except Exception as ex:
		print("データが読み込めませんでした",file=sys.stderr)
		print(ex,file=sys.stderr)

	dataTitle=dataArr.pop(0)
	print(dataTitle)

	data={v:type("",(),{"used":False}) for v in dataArr}

	chars="アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲ"
	lastWord=""
	while True:
		lastWord=random.choice(chars)
		if any(v[0]==lastWord and "ン"!=v[-1] for v in dataArr): break

	cpuMemory=copy(dataArr)

	isUser=0!=random.randrange(2)
	print(f"はじめは{'あなた' if isUser else 'あいて'}からです")
	print(f"最初の文字は「{lastWord}」です\n")

	cpuRetry=3
	while True:
		if not any("ン"!=v[-1] and
				not data[v].used and
				check(lastWord,v)
					for v in dataArr):
			print(f"{'あなた' if isUser else 'あいて'}にもう勝ち目はありません")
			print(f"あなたの{'負け' if isUser else '勝ち'}です")
			break

		ans=input("あなた> ") if isUser else random.choice(cpuMemory)

		if ans=="quit":
			print("ギブアップしました")
			print("あなたの負けです")
			break

		if (ans in data) and check(lastWord,ans):
			if("ン"==ans[-1]):
				if not isUser:
					if 0<cpuRetry:
						cpuRetry-=1
						continue
					else:
						cpuRetry=0
						print("あいて> {ans}")

				print("あなたの{'負け' if isUser else '勝ち'}です")
				break

			if data[ans].used:
				if isUser: print("既に使われています")
				continue
			else:
				if not isUser:
					print(f"あいて> {ans}")
					cpuMemory.remove(ans)
				print("　　　↓")
				data[ans].used=True
				lastWord=ans
				isUser=not isUser
				cpuRetry=3
		else:
			if isUser: print("間違っています")
			continue
	input()
	

if __name__=="__main__": main(sys.argv[1:])

