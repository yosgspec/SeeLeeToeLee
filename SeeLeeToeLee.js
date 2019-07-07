"use strict";
const rl=require("readline").createInterface(process.stdin,process.stdout);
const fs=require("fs");

const smallChars=/[ァィゥェォヵヶッャュョヮ]/;
const toPlainChar=[
	[/ァ/g,    "ア"],[/ィ/g,    "イ"],[/ゥヴ/g,  "ウ"],[/ェ/g,    "エ"],[/ォ/g,    "オ"],
	[/ヵガ/g,  "カ"],[/ギ/g,    "キ"],[/グ/g,    "ク"],[/ヶゲ/g,  "ケ"],[/ゴ/g,    "コ"],
	[/ザ/g,    "サ"],[/ジ/g,    "シ"],[/ズ/g,    "ス"],[/ゼ/g,"    セ"],[/ゾ/g,    "ソ"],
	[/ダ/g,    "タ"],[/ヂ/g,    "チ"],[/ッヅ/g,  "ツ"],[/デ/g,    "テ"],[/ド/g,    "ト"],
	[/[バパ]/g,"ハ"],[/[ビピ]/g,"ヒ"],[/[ブプ]/g,"フ"],[/[ベペ]/g,"ヘ"],[/[ボポ]/g,"ホ"],
	[/ャ/g,    "ヤ"],[/ュ/g,    "ユ"],[/ョ/g,    "ヨ"],[/ヮ/g,    "ワ"],
	[/[2２]/g,"ツ"],[/[ZＺ]/g,"ゼット"],[/♂/g,  "オス"],[/♀/g,  "メス"]
];

function check(lastWord,nextWord){
	lastWord=lastWord.replace(/ー/g,"");
	const isLastSmall=smallChars.test(lastWord[lastWord.length-1]);

	for(let v of toPlainChar){
		lastWord=lastWord.replace(v[0],v[1]);
		nextWord=nextWord.replace(v[0],v[1]);
	}

	if(lastWord[lastWord.length-1]==nextWord[0] ||
		isLastSmall && lastWord.slice(-2)==nextWord.slice(0,2))
			return true;
	return false;
}

const g=function*(){
	var dataArr;
	try{
		dataArr=fs.readFileSync("./data.csv")
			.toString()
			.trim()
			.split(/\r\n|\n|\r/);
	}
	catch(ex){
		console.log("データが読み込めませんでした");
		console.log(ex);
		yield rl.once("line",()=>g.next());
		process.exit();
	}
	const dataTitle=dataArr.shift();
	console.log(dataTitle);

	const data={};
	for(let v of dataArr){
		data[v]={used:false};
	}
	const chars="アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲ";
	var lastWord;
	for(;;){
		lastWord=chars[0|Math.random()*chars.length];
		if(dataArr.some(v=>v[0]==lastWord && "ン"!=v[v.length-1])) break;
	}

	const cpuMemory=Array.from(dataArr);

	var isUser=0!=0|Math.random()*2;
	console.log(`はじめは${isUser?"あなた":"あいて"}からです`);
	console.log(`最初の文字は「${lastWord}」です\n`);
	var cpuRetry=3;
	for(;;){
		if(!dataArr.some(v=>
				"ン"!=v[v.length-1] &&
				!data[v].used &&
				check(lastWord,v))){
			console.log(`${isUser?"あなた":"あいて"}にもう勝ち目はありません`);
			console.log(`あなたの${isUser?"負け":"勝ち"}です`);
			break;
		}
		let ans=isUser?
			yield rl.question("あなた> ",s=>g.next(s)):
			cpuMemory[0|Math.random()*cpuMemory.length];

		if(ans=="quit"){
			console.log("ギブアップしました");
			console.log("あなたの負けです");
			break;
		}
		if(data[ans] && check(lastWord,ans)){
			if("ン"==ans[ans.length-1]){
				if(!isUser){
					if(0<cpuRetry){
						cpuRetry--;
						continue;
					}
					else{
						cpuRetry=0;
						console.log(`あいて> ${ans}`);
					}
				}
				console.log(`あなたの${isUser?"負け":"勝ち"}です`);
				break;
			}
			if(data[ans].used){
				if(isUser) console.log("既に使われています");
				continue;
			}
			else{
				if(!isUser){
					console.log(`あいて> ${ans}`);
					cpuMemory.splice(cpuMemory.indexOf(ans),1);
				}
				console.log("　　　↓");
				data[ans].used=true;
				lastWord=ans;
				isUser=!isUser;
				cpuRetry=3;
			}
		}
		else{
			if(isUser) console.log("間違っています");
			continue;
		}
	}
	rl.once("line",()=>process.exit());
}();
g.next();
