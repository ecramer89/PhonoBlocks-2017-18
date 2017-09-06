﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using UnityEngine;

public static class LetterSoundComponentRegex  {

	//Vowel Regex
	static string[] vowels = new string[]{"a","e","i","o","u"};
	static string vowel = MatchAnyOf (vowels);
	static Regex vowelRegex = Make(vowel);
	public static Regex Vowel{
		get {
			return vowelRegex;
		}
	}


	//Consonant Regex
	static string consonant = $"[^\\W,^{vowel}]";
	static Regex consonantRegex = Make(consonant);
	public static Regex Consonant{
		get {
			return consonantRegex;
		}
	}

	//Consonant Digraph regex
	static string[] consonantDigraphs = new string[]{"th","ch","sh","qu","ck","ng","nk","wh"};
	static string consonantDigraph = MatchAnyOf (consonantDigraphs);
	static Regex consonantDigraphRegex = Make(consonantDigraph);
	public static Regex ConsonantDigraph{
		get {
			return consonantDigraphRegex;
		}
	}
		
	//Consonant Blend Regex
	static string[] consonantBlends = new string[]{
		"sp","sh", "sl", "sk", "str", "spr", "scr", "spl", "squ", "shr",
		"ll", "bl", "gl", "cl", "pl", "fl", "cr", "tr", "dr", "ft", "nd", 
		"mp", "nt","thr"
	};
	static string consonantBlend = MatchAnyOf (consonantBlends);
	static Regex consonantBlendRegex = Make(consonantBlend);
	public static Regex ConsonantBlend{
		get {
			return consonantBlendRegex;
		}
	}

	static string[] vowelDigraphs = new string[] {
		"ea", "ai", "ae", "aa", "ee", "ie", "oe", "ue", "ou", "ay", "oa"
	};
	static string vowelDigraph = MatchAnyOf (vowelDigraphs);
	static Regex vowelDigraphRegex = Make(vowelDigraph);
	public static Regex VowelDigraph{
		get {
			return vowelDigraphRegex;
		}
	}
	static string[] rControlledVowels = new string[]{
		"er", "ur", "or","ir","ar"
	};
	static string rControlledVowel = MatchAnyOf (rControlledVowels);
	static Regex rControlledVowelRegex = Make(rControlledVowel);
	public static Regex RControlledVowel{
		get{
			return rControlledVowelRegex;
		}
	}

	static string anyConsonant = MatchAnyOf(new string[]{consonant, consonantDigraph, consonantBlend});

	//Magic-E rule regex
	static Regex magicERule = Make($"({anyConsonant})({vowel})({anyConsonant})e$");
	public static Regex MagicERule{
		get {
			return magicERule;
		}
	}

	//Open syllable regex
	static Regex openSyllable = Make($"({anyConsonant})?({vowel})");
	//Closed syllable regex
	static Regex closedSyllable = Make($"({anyConsonant})?({vowel})({consonant})[^e]");

	static string MatchAnyOf(string[] patterns){
		return patterns.Aggregate((acc, nxt)=>$"{acc}|{nxt}");
	}
	//convenience 'factory-style' method; just saves me having to remember to add the case insensitive
	//modifer to each of the regexes.
	static Regex Make(string pattern){
		return new Regex (pattern, RegexOptions.IgnoreCase);
	}



	public static void Test(){

		TestMatchMagicERule ();

	}

	static void TestMatchMagicERule(){
		
		Debug.Log ($"Matches game: {MagicERule.IsMatch("game")}"); 
		Debug.Log ($"Matches shame: {MagicERule.IsMatch("shame")}"); 
		Debug.Log ($"Matches ike: {MagicERule.IsMatch("ike")}"); 
		Debug.Log ($"Does not match shanke: {!MagicERule.IsMatch("shanke")}");
		Debug.Log ($"Does not match sa: {!MagicERule.IsMatch("sa")}");
		Debug.Log ($"Does not match as: {!MagicERule.IsMatch("as")}");
		Debug.Log ($"Does not match a: {!MagicERule.IsMatch("as")}");
	}
		

}