using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Netology.Wiki
{
	public class TextFormatter
	{
		private const string UnderlineExpression = @"\_(.*)\_";
		private const string UnderlineReplacement = @"<u>$1</u>";

		private const string BoldExpression = @"\*(.*)\*";
		private const string BoldReplacement = @"<b>$1</b>";

		private const string ItalicExpression = @"\/\/(.*)\/\/";
		private const string ItalicReplacement = @"<i>$1</i>";

		private const string StrikeoutExpression = @"\~(.*)\~";
		private const string StrikeoutReplacement = @"<s>$1</s>";

		private const string SuperExpression = @"\^(.*)\^";
		private const string SuperReplacement = @"<sup>$1</sup>";

		private const string SubExpression = @"\,\,(.*)\,\,";
		private const string SubReplacement = @"<sub>$1</sub>";

		private const string Headline1Expression = @"=(.*)=";
		private const string Headline1Replacement = @"<h1>$1</h1>";

		private const string Headline2Expression = @"==(.*)==";
		private const string Headline2Replacement = @"<h2>$1</h2>";

		private const string Headline3Expression = @"===(.*)===";
		private const string Headline3Replacement = @"<h3>$1</h3>";

		private const string BulletExpression = @"\s\* (.*)$";
		private const string BulletReplacement = @"<li>$1</li>";

		private const string NumberExpression = @"\s\# (.*)$";
		private const string NumberReplacement = @"<li>$1</li>";




        private const string LinkAlternateExpression = @"\[\[([\d\w\-\s\:\.\\\/]*)\|([\d\w\-\s\: \.\\\/\'\?]*)\]\]";
        private const string LinkAlternateReplacement = @"<a href='$1'>$2</a>";

        private const string LinkExpression = @"\[\[([\d\w\\-s\:\.\\\/]*)\]\]";
        private const string LinkReplacement = @"<a href='$1'>$1</a>";

        private const string TableExpression = @"\s\|\|(.*)\|\|[\s]*$";
        private const string TableReplacement = @"{ $1 } ";


		private readonly Dictionary<string, string> Replacements;

		public TextFormatter()
        {
            Replacements = new Dictionary<string, string>();

            Replacements.Add(UnderlineExpression, UnderlineReplacement);
            Replacements.Add(BoldExpression, BoldReplacement);
            Replacements.Add(ItalicExpression, ItalicReplacement);
            Replacements.Add(StrikeoutExpression, StrikeoutReplacement);
            Replacements.Add(SuperExpression, SuperReplacement);
            Replacements.Add(SubExpression, SubReplacement);
            Replacements.Add(LinkExpression, LinkReplacement);
            Replacements.Add(LinkAlternateExpression, LinkAlternateReplacement);
        }

		public string Prepare(string Page)
        {
            string Output = Page;
			Output = MakeTable(Output);
			Output = MakeNumbered(Output);
			Output = MakeBulleted(Output);

			//Generals....
            foreach (KeyValuePair<string,string> item in Replacements)
            {
                Output = Regex.Replace(Output, item.Key, item.Value);
            }

            Output = Regex.Replace(Output, Headline3Expression, Headline3Replacement);
            Output = Regex.Replace(Output, Headline2Expression, Headline2Replacement);
           	Output = Regex.Replace(Output, Headline1Expression, Headline1Replacement);

            return Output;
        }

		private string MakeBulleted(string Output)
		{
			string[] lines = Output.Split(new char[] {'\n'});
			bool hasBullet = false;
			string bullets = "";
			Output = "";

			foreach (string line in lines)
			{
		       	if (Regex.IsMatch(line, BulletExpression))
			   	{
					if (!hasBullet)
					{
						hasBullet = true;
					}

					bullets += Regex.Replace(line, BulletExpression, BulletReplacement)+"\n";
			   	}
			   	else
			   	{
					if (hasBullet)
					{
						hasBullet = false;
						Output += string.Format("<ul>{0}</ul>", bullets);
						bullets = "";
					}

			   		Output += line+"\n";
			   	}		  
			}

			return Output;
		}

		private string MakeNumbered(string Output)
		{
			string[] lines = Output.Split(new char[] {'\n'});
			bool hasNumbered = false;
			string numbers = "";
			Output = "";

			foreach (string line in lines)
			{
		       	if (Regex.IsMatch(line, NumberExpression))
			   	{
					if (!hasNumbered)
					{
						hasNumbered = true;
					}

					numbers += Regex.Replace(line, NumberExpression, NumberReplacement)+"\n";
			   	}
			   	else
			   	{
					if (hasNumbered)
					{
						hasNumbered = false;
						Output += string.Format("<ol>{0}</ol>", numbers);
						numbers = "";
					}

			   		Output += line+"\n";
			   	}		  
			}

			return Output;

		}


		private string MakeTable(string Output)
		{
			// Make Table 
			string[] lines = Output.Split(new char[] {'\n'});
			bool hasTable = false;

			Output = "";
			int columnSize = 0;
			List<string[]> rows = new List<string[]>();
			foreach (string line in lines)
			{
		       	if (Regex.IsMatch(line, TableExpression))
			   	{
					if (!hasTable)
					{
						hasTable = true;
					}

					string[] columns = Regex.Split(line, @"\|\|");

					if (columnSize < columns.Length) 
						columnSize = columns.Length;

					rows.Add(columns);
			   	}
			   	else
			   	{
					if (hasTable)
					{
						Output += "<table>\n";
						foreach (string[] columns in rows)
						{
							Output += "<tr>\n";

							for (int i = 1; i < columnSize -1; i++)
							{
								Output += (i < columns.Length) ? "<td>"+columns[i]+"</td>\n" : "<td></td>\n";
							}

							Output += "</tr>\n";
						}
						Output += "</table>\n";

						hasTable = false;
						columnSize = 0;
						rows.Clear();
					}

			   		Output += line+"\n";
			   	}		  
			}

			return Output;
		}


		public static void Main(string[] args)
		{
			string text = @"
             = Adım Adım Wiki =
                
                == Wiki Nedir? ==
                
                1994'ten beri ayakta olan internet uzerinde calışan *hypertext* veritabanı. 
                Metin formatındaki girdilerin tag kullanmadan özel işaretlerle taglenmesidir.
                
                === Başlıca Metin Simgeleri ===

				*Bold text: Kalın metin* <br/>
				//Italic text: Yatık metin// <br/>
				_Underline text: Alt çizgili metin_
				~Strikeout text: üzeri çizili metin ~
				[[/projects/njoy|njoy link]]
				[[http://www.google.com|google]]

                === Keywords ===
                || alias || and || BEGIN ||
                || begin || break || case ||    
                || class || def || defined ||


				=== QualityRank ===
				* Çekirdek
				* Performans
				* Kullanılabilirlik
				
				=== QualityRank ===
                # Çekirdek
                # Performans
                # Kullanılabilirlik
                # Hedefler
                # İçerik
                # İstatistik
					";

			TextFormatter wiki = new TextFormatter();
			System.Console.WriteLine(wiki.Prepare(text));
		}
	}
}
