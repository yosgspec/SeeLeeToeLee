Option Strict On
Option Infer On
Imports System.IO
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions

Class DataItem
	Public used As Boolean
	Public Sub New(used As Boolean)
		Me.used=used
	End Sub 
End Class

Module Program
	Dim rand As New Random()
	Dim smallChars As String="ァィゥェォヵヶッャュョヮ"
	Dim toPlainChar As (IEnumerable(Of String),String)()=Array.ConvertAll({
		("ァ",  "ア"),("ィ",  "イ"),("ゥヴ","ウ"),("ェ",  "エ"),("ォ",  "オ"),
		("ヵガ","カ"),("ギ",  "キ"),("グ",  "ク"),("ヶゲ","ケ"),("ゴ",  "コ"),
		("ザ",  "サ"),("ジ",  "シ"),("ズ",  "ス"),("ゼ",  "セ"),("ゾ",  "ソ"),
		("ダ",  "タ"),("ヂ",  "チ"),("ッヅ","ツ"),("デ",  "テ"),("ド",  "ト"),
		("バパ","ハ"),("ビピ","ヒ"),("ブプ","フ"),("ベペ","ヘ"),("ボポ","ホ"),
		("ャ",  "ヤ"),("ュ",  "ユ"),("ョ",  "ヨ"),("ヮ",  "ワ"),
		("2２", "ツ"),("Zゼット", "ゼット"),("♂","オス"),("♀","メス")
	},Function(v) (v.Item1.ToList().Select(Function(s) s.ToString()),v.Item2))

	Function check(lastWord As String,nextWord As String) As Boolean
		lastWord=lastWord.Replace("ー","")
		Dim isLastSmall=smallChars.Contains(lastWord(lastWord.Length-1).ToString())

		For Each v In toPlainChar
			lastWord=v.Item1.Aggregate(lastWord,Function(s,sep) s.Replace(sep,v.Item2))
			nextWord=v.Item1.Aggregate(nextWord,Function(s,sep) s.Replace(sep,v.Item2))
		Next

		If lastWord(lastWord.Length-1)=nextWord(0) OrElse
			isLastSmall AndAlso Right(lastWord,2)=Left(nextWord,2) Then _
				Return True
		Return False
	End Function

	Sub Main()
		Dim dataArr As String()
		Try
			dataArr=File.ReadAllText("./data.csv") _
				.Trim() _
				.Replace(vbCrLf,vbLf) _
				.Split({vbLf(0),vbCr(0)})
		Catch ex As Exception
			Console.WriteLine("データが読み込めませんでした")
			Console.WriteLine(ex)
			Console.ReadLine()
			Return
		End Try

		Dim dataTitle=dataArr(0)
		Console.WriteLine(dataTitle)
		If True Then
			Dim a(dataArr.Length-2) As String
			Array.Copy(dataArr,1,a,0,a.Length)
			dataArr=a
		End If

		Dim data As New Dictionary(Of String,DataItem)
		For Each v In dataArr
			data.Add(v,New DataItem(False))
		Next

		Dim chars="アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲ"
		Dim lastWord As String
		Do
			lastWord=chars(rand.Next(chars.Length)).ToString()
			If Array.Exists(dataArr,Function(v) v(0).ToString()=lastWord AndAlso "ン"<>v(v.Length-1)) Then Exit Do
		Loop

		Dim cpuMemory=New List(Of String)(dataArr)

		Dim isUser=0<>rand.Next(2)
		Console.WriteLine($"はじめは{If(isUser,"あなた","あいて")}からです")
		Console.WriteLine($"最初の文字は「{lastWord}」です\n")
		Dim cpuRetry=3
		Do
			If Not Array.Exists(dataArr,Function(v) _
					"ン"<>v(v.Length-1) AndAlso
					Not data(v).used AndAlso
					check(lastWord,v)) Then
				Console.WriteLine($"{If(isUser,"あなた","あいて")}にもう勝ち目はありません")
				Console.WriteLine($"あなたの{If(isUser,"負け","勝ち")}です")
				Exit Do
			End If

			If isUser Then Console.Write("あなた> ")
			Dim ans=If(isUser,
				Console.ReadLine(),
				cpuMemory(rand.Next(cpuMemory.Count)))

			If ans="quit" Then
				Console.WriteLine("ギブアップしました")
				Console.WriteLine("あなたの負けです")
				Exit Do
			End If
	
			If data.ContainsKey(ans) AndAlso check(lastWord,ans) Then
				If "ン"=ans(ans.Length-1) Then
					If Not isUser Then
						If 0<cpuRetry Then
							cpuRetry-=1
							Continue Do
						Else
							cpuRetry=0
							Console.WriteLine("あいて >{ans}")
						End If
					End If
					Console.WriteLine($"あなたの{If(isUser,"負け","勝ち")}です")
					Exit Do
				End If
				If data(ans).used Then
					If isUser Then Console.WriteLine("既に使われています")
					Continue Do
				Else
					If Not isUser Then
						Console.WriteLine($"あいて> {ans}")
						cpuMemory.Remove(ans)
					End If
					Console.WriteLine("　　　↓")
					data(ans).used=True
					lastWord=ans
					isUser=Not isUser
					cpuRetry=3
				End If
			Else
				If isUser Then Console.WriteLine("間違っています")
				Continue Do
			End If
		Loop
		Console.ReadLine()
	End Sub
End Module
