﻿using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AniDB.Tests {
	[TestFixture]
	class AnimeTests
	{
		const string Request = "ANIME aid=1&amask=b2f0e0fc000000&s=xxxxx";
		const string ResponseString = "230 ANIME\n1|1999-1999|TV Series|Space,Future,Plot Continuity,SciFi,Space Travel,Shipboard,Other Planet,Novel,Genetic Modification,Action,Romance,Military,Large Breasts,Gunfights,Adventure,Human Enhancement,Nudity|Seikai no Monshou|星界の紋章|Crest of the Stars||13|13|3|853|3225|756|110|875|11";

		private AniDBResponse _response;
		private Anime.AMask _aMask;
		private Anime _anime;

		[SetUp]
		public void SetUp()
		{   
			_response = new AniDBResponse(Encoding.UTF8.GetBytes(ResponseString));

			//I think I'm probably overengineering this; why not just store the amask as 
			// a const string instead of the entire request? It's the only part (of the request)
			// that's relevant to testing the Anime class
			_aMask = new Anime.AMask((Anime.AMask.AMaskValues)ulong.Parse(new Regex(@"(?<=amask\=)\w+((?=\&.+)|$)").Match(Request).Value, NumberStyles.HexNumber));

			_anime = new Anime(_response, _aMask);

			for (int i = 0; i < _response.DataFields[0].Length; i++ )
				Console.WriteLine(String.Format("{0,3}", "[" + i) + "] " + _response.DataFields[0][i]);

			Console.WriteLine();

			Console.WriteLine(_aMask.MaskString);

			for (int i = 55; i >= 0; i--)
			{
				Anime.AMask.AMaskValues flag = (Anime.AMask.AMaskValues)(ulong)Math.Pow(2, i);
				if (_aMask.Mask.HasFlag(flag))
					Console.WriteLine(flag);
			}
		}

		[Test]
		public void AID()
		{
			Assert.That(_anime.AID, Is.EqualTo(int.Parse(_response.DataFields[0][0])));
		}
		
		[Test]
		public void Year()
		{
			Assert.That(_anime.Year, Is.EqualTo(_response.DataFields[0][1]));
		}

		[Test]
		public void Type()
		{
			Assert.That(_anime.Type, Is.EqualTo(_response.DataFields[0][2]));
		}

		[Test]
		public void CategoryList()
		{
			Assert.That(_anime.CategoryList, Is.Not.Null);

			string[] sa = _response.DataFields[0][3].Split(',');
			for (int i = 0; i < sa.Length; i++)
				Assert.That(_anime.CategoryList[i], Is.EqualTo(sa[i]));
		}

		[Test]
		public void RomanjiName()
		{
			Assert.That(_anime.RomanjiName, Is.EqualTo(_response.DataFields[0][4]));
		}

		[Test]
		public void KanjiName()
		{
			Assert.That(_anime.KanjiName, Is.EqualTo(_response.DataFields[0][5]));
		}

		[Test]
		public void EnglishName()
		{
			Assert.That(_anime.EnglishName, Is.EqualTo(_response.DataFields[0][6]));
		}

		[Test]
		public void OtherName()
		{
			//TODO: Fix this, this can (probably will) be null...
			Assert.That(_anime.OtherName, Is.Not.Null);
		}

		[Test]
		public void Episodes()
		{
			Assert.That(_anime.Episodes, Is.EqualTo(int.Parse(_response.DataFields[0][8])));
		}

		[Test]
		public void HighestEpisodeNumber()
		{
			Assert.That(_anime.HighestEpisodeNumber, Is.EqualTo(int.Parse(_response.DataFields[0][9])));
		}

		[Test]
		public void SpecialEpCount()
		{
			Assert.That(_anime.SpecialEpCount, Is.EqualTo(int.Parse(_response.DataFields[0][10])));
		}

		[Test]
		public void Rating()
		{
			Assert.That(_anime.Rating, Is.EqualTo(int.Parse(_response.DataFields[0][11])));
		}

		[Test]
		public void VoteCount()
		{
			Assert.That(_anime.VoteCount, Is.EqualTo(int.Parse(_response.DataFields[0][12])));
		}

		[Test]
		public void TempRating()
		{
			Assert.That(_anime.TempRating, Is.EqualTo(int.Parse(_response.DataFields[0][13])));
		}

		[Test]
		public void TempVoteCount()
		{
			Assert.That(_anime.TempVoteCount, Is.EqualTo(int.Parse(_response.DataFields[0][14])));
		}

		[Test]
		public void AverageReviewRating()
		{
			Assert.That(_anime.AverageReviewRating, Is.EqualTo(int.Parse(_response.DataFields[0][15])));
		}

		[Test]
		public void ReviewCount()
		{
			Assert.That(_anime.ReviewCount, Is.EqualTo(int.Parse(_response.DataFields[0][16])));
		}
	}

	[TestFixture]
	class AMaskTests
	{
		[Test]
		public void TestMaskStringLength()
		{
			Assert.That(new Anime.AMask(Anime.AMask.AMaskValues.None).MaskString.Length == 14);
		}

		[Test]
		public void TestAllMaskValues()
		{
			Anime.AMask.AMaskValues allValues = Anime.AMaskNames.Keys.Aggregate(
				Anime.AMask.AMaskValues.None, (current, a) => current | a);

			const ulong expectedValue = (
											((ulong) (128 | 32 | 16 | 8 | 4 | 2 | 1) << 8*6) |
											((ulong) (128 | 64 | 32 | 16 | 8 | 4) << 8*5) |
											((ulong) (128 | 64 | 32 | 16 | 8 | 4 | 2 | 1) << 8*4) |
											((ulong) (128 | 64 | 32 | 16 | 8 | 4 | 2 | 1) << 8*3) |
											((ulong) (128 | 64 | 32 | 16 | 1) << 8*2) |
											((ulong) (128 | 64 | 32 | 16) << 8*1) |
											((128 | 64 | 32 | 16 | 8)));

			Console.WriteLine("Expected:\n" + ((ulong)allValues).ToString("x") + "\nActual:\n" + expectedValue.ToString("x"));

			Assert.That((ulong)allValues == expectedValue);
		}
	}
}