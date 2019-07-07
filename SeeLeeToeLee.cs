using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

class DataItem{
	public bool used;
	public DataItem(bool used){this.used=used;}
}

class Program{
	static Random rand=new Random();
	static string smallChars="ァィゥェォヵヶッャュョヮ";
	static (IEnumerable<string>,string)[] toPlainChar=Array.ConvertAll(new[]{
		("ァ",  "ア"),("ィ",  "イ"),("ゥヴ","ウ"),("ェ",  "エ"),("ォ",  "オ"),
		("ヵガ","カ"),("ギ",  "キ"),("グ",  "ク"),("ヶゲ","ケ"),("ゴ",  "コ"),
		("ザ",  "サ"),("ジ",  "シ"),("ズ",  "ス"),("ゼ",  "セ"),("ゾ",  "ソ"),
		("ダ",  "タ"),("ヂ",  "チ"),("ッヅ","ツ"),("デ",  "テ"),("ド",  "ト"),
		("バパ","ハ"),("ビピ","ヒ"),("ブプ","フ"),("ベペ","ヘ"),("ボポ","ホ"),
		("ャ",  "ヤ"),("ュ",  "ユ"),("ョ",  "ヨ"),("ヮ",  "ワ"),
		("2２", "ツ"),("Zゼット", "ゼット"),("♂","オス"),("♀","メス")
	},v=>(v.Item1.ToList().Select(s=>s.ToString()),v.Item2));
	static Func<string,int,string> RightString=(s,len)=>s.Substring(s.Length-len,len);

	static bool check(string lastWord,string nextWord){
		lastWord=lastWord.Replace("ー","");
		var isLastSmall=smallChars.Contains(lastWord[lastWord.Length-1].ToString());

		foreach(var v in toPlainChar){
			lastWord=v.Item1.Aggregate(lastWord,(s,sep)=>s.Replace(sep,v.Item2));
			nextWord=v.Item1.Aggregate(nextWord,(s,sep)=>s.Replace(sep,v.Item2));
		};

		if(lastWord[lastWord.Length-1]==nextWord[0] ||
			isLastSmall && RightString(lastWord,2)==nextWord.Substring(0,2))
				return true;
		return false;
	}

	static void Main(){
		string[] dataArr;
		try{
			dataArr=File.ReadAllText("./data.csv")
				.Trim()
				.Replace("\r\n","\n")
				.Split(new[]{'\n','\r'});
		}
		catch(Exception ex){
			Console.WriteLine("データが読み込めませんでした");
			Console.WriteLine(ex);
			Console.ReadLine();
			return;
		}
		var dataTitle=dataArr[0];
		Console.WriteLine(dataTitle);
		{
			var a=new string[dataArr.Length-1];
			Array.Copy(dataArr,1,a,0,a.Length);
			dataArr=a;
		}

		var data=new Dictionary<string,DataItem>();
		foreach(var v in dataArr){
			data.Add(v,new DataItem(false));
		}
		var chars="アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲ";
		string lastWord;
		for(;;){
			lastWord=chars[rand.Next(chars.Length)].ToString();
			if(Array.Exists(dataArr,v=>v[0].ToString()==lastWord && 'ン'!=v[v.Length-1])) break;
		}

		var cpuMemory=new List<string>(dataArr);

		var isUser=0!=rand.Next(2);
		Console.WriteLine($"はじめは{(isUser?"あなた":"あいて")}からです");
		Console.WriteLine($"最初の文字は「{lastWord}」です\n");
		var cpuRetry=3;
		for(;;){
			if(!Array.Exists(dataArr,v=>
					'ン'!=v[v.Length-1] &&
					!data[v].used &&
					check(lastWord,v))){
				Console.WriteLine($"{(isUser?"あなた":"あいて")}にもう勝ち目はありません");
				Console.WriteLine($"あなたの{(isUser?"負け":"勝ち")}です");
				break;
			}
			if(isUser) Console.Write("あなた> ");
			var ans=isUser?
				Console.ReadLine():
				cpuMemory[rand.Next(cpuMemory.Count)];

			if(ans=="quit"){
				Console.WriteLine("ギブアップしました");
				Console.WriteLine("あなたの負けです");
				break;
			}
			if(data.ContainsKey(ans) && check(lastWord,ans)){
				if('ン'==ans[ans.Length-1]){
					if(!isUser){
						if(0<cpuRetry){
							cpuRetry--;
							continue;
						}
						else{
							cpuRetry=0;
							Console.WriteLine($"あいて> {ans}");
						}
					}
					Console.WriteLine($"あなたの{(isUser?"負け":"勝ち")}です");
					break;
				}
				if(data[ans].used){
					if(isUser) Console.WriteLine("既に使われています");
					continue;
				}
				else{
					if(!isUser){
						Console.WriteLine($"あいて> {ans}");
						cpuMemory.Remove(ans);
					}
					Console.WriteLine("　　　↓");
					data[ans].used=true;
					lastWord=ans;
					isUser=!isUser;
					cpuRetry=3;
				}
			}
			else{
				if(isUser) Console.WriteLine("間違っています");
				continue;
			}
		}
		Console.ReadLine();
	}
}
