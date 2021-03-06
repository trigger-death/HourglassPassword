﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using HourglassPass.Internal;

namespace HourglassPass {
	/// <summary>
	///  A helper structure for easily changing between Scene IDs, Letters, strings, and integers.
	/// </summary>
	[Serializable]
	public struct SceneId : IEquatable<SceneId>, IEquatable<PasswordSceneId>, ILetterString,
		IComparable, IComparable<SceneId>, IComparable<PasswordSceneId>, IComparable<string>, IComparable<int>
	{
		#region Constants

		/// <summary>
		///  The minimum value representable by a Scene ID.
		/// </summary>
		public const int MinValue = PasswordSceneId.MinValue;
		/// <summary>
		///  The maximum value representable by a Scene ID.
		/// </summary>
		public const int MaxValue = PasswordSceneId.MaxValue;
		/// <summary>
		///  The number of letters in this password structure.
		/// </summary>
		public const int Length = PasswordSceneId.Length;

		#region ILetterString Constants

		int IReadOnlyLetterString.MinValue => Length;
		int IReadOnlyLetterString.MaxValue => Length;
		int IReadOnlyCollection<Letter>.Count => Length;

		#endregion

		#endregion

		#region Fields

		/// <summary>
		///  The actual value of the Scene ID.
		/// </summary>
		private int value;

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs a Scene ID with an array of <see cref="Length"/> letters.
		/// </summary>
		/// <param name="letters">The array containing the letters of the Scene ID.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="letters"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  The length of <paramref name="letters"/> is not <see cref="Length"/>.
		/// </exception>
		public SceneId(Letter[] letters) {
			ValidateLetters(letters, nameof(letters));
			value = CopyFromLetters(letters);
		}
		/// <summary>
		///  Constructs a Scene ID with a string of <see cref="Length"/> letters.
		/// </summary>
		/// <param name="scene">The string containing the letters of the Scene ID.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="scene"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  The length of <paramref name="scene"/> is not <see cref="Length"/>.-or- A character in
		///  <paramref name="scene"/> is not a valid letter character.
		/// </exception>
		public SceneId(string scene) {
			ValidateString(ref scene, nameof(scene));
			value = CopyFromString(scene);
		}
		/// <summary>
		///  Constructs a Scene ID with a numeric value.
		/// </summary>
		/// <param name="value">The value of the Scene ID.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="value"/> is less than <see cref="MinValue"/> or greater than <see cref="MaxValue"/>.
		/// </exception>
		public SceneId(int value) {
			ValidateValue(value, nameof(value));
			this.value = value;
		}
		/// <summary>
		///  Constructs a copy of the Password Scene ID.
		/// </summary>
		/// <param name="sceneId">The Password Scene ID to construct a copy of.</param>
		public SceneId(PasswordSceneId sceneId) {
			if (sceneId == null)
				throw new ArgumentNullException(nameof(sceneId));
			value = CopyFromLetters(sceneId.Letters);
		}

		#endregion

		#region Properties

		/// <summary>
		///  Gets or sets the letter at the specified index in the Scene ID.
		/// </summary>
		/// <param name="index">The index of the letter.</param>
		/// <returns>The letter at the specified index in the Scene ID.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Length"/>.
		/// </exception>
		public Letter this[int index] {
			get {
				if (index < 0 || index >= Length)
					throw new ArgumentOutOfRangeException(nameof(index), index,
						$"Index must be between 0 and {(Length-1)}, got {index}!");
				return new Letter((value >> (index * Letter.ShiftValue)) & Letter.MaskValue, false);
			}
			set {
				if (index < 0 || index >= Length)
					throw new ArgumentOutOfRangeException(nameof(index), index,
						$"Index must be between 0 and {(Length-1)}, got {index}!");
				this.value &= ~(Letter.MaskValue >> (index * Letter.ShiftValue));
				this.value |= (value.Value << (index * Letter.ShiftValue));
			}
		}
		/// <summary>
		///  Gets or sets the Scene ID with an array of <see cref="Length"/> letters.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <see cref="Letters"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  The length of <see cref="Letters"/> is not <see cref="Length"/>.
		/// </exception>
		public Letter[] Letters {
			get {
				Letter[] letters = new Letter[3];
				for (int i = 0; i < Length; i++)
					letters[i].Value = (value >> (i * Letter.ShiftValue)) & Letter.MaskValue;
				return letters;
			}
			set {
				ValidateLetters(value, nameof(Letters));
				CopyFromLetters(value);
			}
		}
		/// <summary>
		///  Gets or sets the Scene ID with a string of <see cref="Length"/> letters.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <see cref="String"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  The length of <see cref="String"/> is not <see cref="Length"/>.-or- A character in <see cref="String"/> is
		///  not a valid letter character.
		/// </exception>
		public string String {
			get => string.Join("", Letters);
			set {
				ValidateString(ref value, nameof(String));
				CopyFromString(value);
			}
		}
		/// <summary>
		///  Gets or sets the Scene ID with a numeric value.
		/// </summary>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <see cref="Value"/> is less than <see cref="MinValue"/> or greater than <see cref="MaxValue"/>.
		/// </exception>
		public int Value {
			get => value;
			set {
				ValidateValue(value, nameof(Value));
				this.value = value;
			}
		}

		#endregion

		#region Object Overrides

		/// <summary>
		///  Gets the string representation of the Scene ID's value.
		/// </summary>
		/// <returns>The string representation of the Scene ID's value.</returns>
		public override string ToString() => value.ToString();

		/// <summary>
		///  Gets the string representation of the Scene ID with the specified formatting.
		/// </summary>
		/// <param name="format">
		///  The format to display the letter string in.<para/>
		///  Password: P(Format)[spacing]. Format: S/s = Default, C/c = Corrected, N/n = Normalize, R/r = Randomize, B/b = Binary, D/d = Decimal, X/x = Hexidecimal.<para/>
		///  Binary: VB[spacing] = Binary value format.<para/>
		///  Value: V[format] = Integer value format.
		/// </param>
		/// <returns>The formatted string representation of the Scene ID.</returns>
		/// 
		/// <exception cref="FormatException">
		///  <paramref name="format"/> is invalid.
		/// </exception>
		public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
		/// <summary>
		///  Gets the string representation of the Scene ID with the specified formatting.
		/// </summary>
		/// <param name="format">
		///  The format to display the letter string in.<para/>
		///  Password: P(Format)[spacing]. Format: S/s = Default, C/c = Corrected, N/n = Normalize, R/r = Randomize, B/b = Binary, D/d = Decimal, X/x = Hexidecimal.<para/>
		///  Binary: VB[spacing] = Binary value format.<para/>
		///  Value: V[format] = Integer value format.
		/// </param>
		/// <param name="formatProvider">Unused.</param>
		/// <returns>The formatted string representation of the Scene ID.</returns>
		/// 
		/// <exception cref="FormatException">
		///  <paramref name="format"/> is invalid.
		/// </exception>
		public string ToString(string format, IFormatProvider formatProvider) {
			if (format == null || format.Length == 0)
				return ToString();
			return this.Format(format, formatProvider, "Scene ID");
		}

		/// <summary>
		///  Gets the hash code as the Scene ID's value.
		/// </summary>
		/// <returns>The Scene ID's value.</returns>
		public override int GetHashCode() => value;

		/// <summary>
		///  Checks if the object is a <see cref="SceneId"/>, <see cref="PasswordSceneId"/>, <see cref="Letter"/>[],
		///  <see cref="string"/>, or <see cref="int"/> and Checks for equality between the values of the letter
		///  strings.
		/// </summary>
		/// <param name="obj">The object to check for equality with.</param>
		/// <returns>The object is a compatible type and has the same value as this letter string.</returns>
		public override bool Equals(object obj) {
			if (ReferenceEquals(this, obj)) return true;
			if (obj is SceneId id) return Equals(id);
			if (obj is PasswordSceneId pwid) return Equals(pwid);
			if (obj is Letter[] l) return Equals(l);
			if (obj is string s) return Equals(s);
			if (obj is int i) return Equals(i);
			return false;
		}
		/// <summary>
		///  Checks for equality between the values of the letter strings.
		/// </summary>
		/// <param name="other">The letter string to check for equality with.</param>
		/// <returns>The letter string has the same value as this letter string.</returns>
		public bool Equals(SceneId other) => Value == other.Value;
		/// <summary>
		///  Checks for equality between the values of the letter strings.
		/// </summary>
		/// <param name="other">The letter string to check for equality with.</param>
		/// <returns>The letter string has the same value as this letter string.</returns>
		public bool Equals(PasswordSceneId other) => other != null && Value == other.Value;
		/// <summary>
		///  Checks for equality between the value of the letter string and that of the letter array.
		/// </summary>
		/// <param name="other">The letter array to check for equality with values.</param>
		/// <returns>The letter array has the same value as this letter string.</returns>
		public bool Equals(Letter[] other) => other != null && Value == new PasswordSceneId(other).Value;
		/// <summary>
		///  Checks for equality between the value of the letter string and that of the string.
		/// </summary>
		/// <param name="other">The string to check for equality with values.</param>
		/// <returns>The string has the same value as this letter string.</returns>
		public bool Equals(string other) => other != null && Value == new PasswordSceneId(other).Value;
		/// <summary>
		///  Compares the value with that of this letter string.
		/// </summary>
		/// <param name="other">The value to check for equality with.</param>
		/// <returns>The value is the same as this letter string's value.</returns>
		public bool Equals(int other) => Value == other;

		/// <summary>
		///  Checks if the object is a <see cref="SceneId"/>, <see cref="PasswordSceneId"/>, <see cref="Letter"/>[],
		///  <see cref="string"/>, or <see cref="int"/> and compares the values.
		/// </summary>
		/// <param name="obj">The object to compare values with.</param>
		/// <returns>The comparison of the two objects.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="obj"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="obj"/> is not a <see cref="SceneId"/>, <see cref="PasswordSceneId"/>,
		///  <see cref="Letter"/>[], <see cref="string"/>, or <see cref="int"/>.
		/// </exception>
		public int CompareTo(object obj) {
			if (obj is SceneId id) return CompareTo(id);
			if (obj is PasswordSceneId pwid) return CompareTo(pwid);
			if (obj is Letter[] l) return CompareTo(l);
			if (obj is string s) return CompareTo(s);
			if (obj is int i) return CompareTo(i);
			if (obj is null)
				throw new ArgumentNullException(nameof(obj));
			throw new ArgumentException($"Scene ID cannot be compared against type {obj.GetType().Name}!");
		}
		/// <summary>
		///  Compares the values of the letter strings.
		/// </summary>
		/// <param name="obj">The letter string to compare with.</param>
		/// <returns>The comparison of the two letter strings.</returns>
		public int CompareTo(SceneId other) => Value.CompareTo(other.Value);
		/// <summary>
		///  Compares the values of the letter strings.
		/// </summary>
		/// <param name="obj">The letter string to compare with.</param>
		/// <returns>The comparison of the two letter strings.</returns>
		public int CompareTo(PasswordSceneId other) => Value.CompareTo(other.Value);
		/// <summary>
		///  Compares the values of the letter string and letter array.
		/// </summary>
		/// <param name="obj">The letter array to compare with.</param>
		/// <returns>The comparison of the letter string and letter array.</returns>
		public int CompareTo(Letter[] other) => Value.CompareTo(new PasswordSceneId(other).Value);
		/// <summary>
		///  Compares the values of the letter string and string.
		/// </summary>
		/// <param name="obj">The string to compare with.</param>
		/// <returns>The comparison of the letter string and string.</returns>
		public int CompareTo(string other) => Value.CompareTo(new PasswordSceneId(other).Value);
		/// <summary>
		///  Compares the value with the letter string's value.
		/// </summary>
		/// <param name="obj">The value to compare with.</param>
		/// <returns>The comparison of the letter string and value.</returns>
		public int CompareTo(int other) => Value.CompareTo(other);

		#endregion

		#region Parse

		/// <summary>
		///  Parses the string representation of the Scene ID.
		/// </summary>
		/// <param name="s">The string representation of the Scene ID.</param>
		/// <returns>The parsed Scene ID.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="s"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="s"/> is not a valid Scene ID.
		/// </exception>
		public static SceneId Parse(string s) {
			return Parse(s, PasswordStyles.PasswordOrValue);
		}
		/// <summary>
		///  Parses the string representation of the Scene ID.
		/// </summary>
		/// <param name="s">The string representation of the Scene ID.</param>
		/// <param name="style">The style to parse the Scene ID in.</param>
		/// <returns>The parsed Scene ID.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="s"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="s"/> is not a valid Scene ID.-or-<paramref name="style"/> is not a valid
		///  <see cref="PasswordStyles"/>.
		/// </exception>
		public static SceneId Parse(string s, PasswordStyles style) {
			Letter[] letters = LetterUtils.ParseLetterString(s, style, "Scene ID", Length, out int value);
			return (letters != null ? new SceneId(letters) : new SceneId(value));
		}
		/// <summary>
		///  Parses the string representation of the Scene ID's value.
		/// </summary>
		/// <param name="s">The string representation of the Scene ID.</param>
		/// <param name="style">The style to parse the Scene ID's value in.</param>
		/// <returns>The parsed Scene ID.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="s"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="s"/> is not a valid Scene ID.-or-<paramref name="style"/> is not a valid
		///  <see cref="NumberStyles"/>.
		/// </exception>
		/// <exception cref="FormatException">
		///  <paramref name="s"/> does not follow the number format.
		/// </exception>
		public static SceneId Parse(string s, NumberStyles style) {
			return new SceneId(int.Parse(s, style));
		}

		/// <summary>
		///  Tries to parse the string representation of the Scene ID.
		/// </summary>
		/// <param name="s">The string representation of the Scene ID.</param>
		/// <param name="sceneId">The output Scene ID on success.</param>
		/// <returns>True if the Scene ID was successfully parsed, otherwise false.</returns>
		public static bool TryParse(string s, out SceneId sceneId) {
			return TryParse(s, PasswordStyles.PasswordOrValue, out sceneId);
		}
		/// <summary>
		///  Tries to parse the string representation of the Scene ID.
		/// </summary>
		/// <param name="s">The string representation of the Scene ID.</param>
		/// <param name="style">The style to parse the Scene ID in.</param>
		/// <param name="sceneId">The output Scene ID on success.</param>
		/// <returns>True if the Scene ID was successfully parsed, otherwise false.</returns>
		public static bool TryParse(string s, PasswordStyles style, out SceneId sceneId) {
			if (LetterUtils.TryParseLetterString(s, style, "Scene ID", Length, out Letter[] letters, out int value)) {
				sceneId = (letters != null ? new SceneId(letters) : new SceneId(value));
				return true;
			}
			sceneId = new SceneId();
			return false;
		}
		/// <summary>
		///  Tries to parse the string representation of the Scene ID's value.
		/// </summary>
		/// <param name="s">The string representation of the Scene ID's value.</param>
		/// <param name="style">The style to parse the Scene ID's value in.</param>
		/// <param name="sceneId">The output Scene ID on success.</param>
		/// <returns>True if the Scene ID was successfully parsed, otherwise false.</returns>
		public static bool TryParse(string s, NumberStyles style, out SceneId sceneId) {
			if (int.TryParse(s, style, CultureInfo.CurrentCulture, out int value)) {
				sceneId = new SceneId(value);
				return true;
			}
			sceneId = new SceneId();
			return false;
		}

		#endregion

		#region ILetterString Mutate

		/// <summary>
		///  Returns a Scene ID with normalized interchangeable characters.
		/// </summary>
		/// <param name="garbageChar">The character to use for garbage letters.</param>
		/// <returns>The normalized Scene ID with consistent interchangeable characters.</returns>
		public SceneId Normalized(char garbageChar = Letter.GarbageChar) => this;
		/// <summary>
		///  Returns a Scene ID with randomized interchangeable characters.
		/// </summary>
		/// <returns>The randomized Scene ID with random interchangable characters.</returns>
		public SceneId Randomized() => this;
		ILetterString ILetterString.Normalized(char garbageChar) => Normalized(garbageChar);
		ILetterString ILetterString.Randomized() => Randomized();
		IReadOnlyLetterString IReadOnlyLetterString.Normalized(char garbageChar) => Normalized(garbageChar);
		IReadOnlyLetterString IReadOnlyLetterString.Randomized() => Randomized();

		/// <summary>
		///  Normalizes the Scene ID's interchangeable characters.
		/// </summary>
		/// <param name="garbageChar">The character to use for garbage letters.</param>
		public void Normalize(char garbageChar = Letter.GarbageChar) { /* Do nothing */ }
		/// <summary>
		///  Randomizes the Scene ID's interchangeable characters.
		/// </summary>
		public void Randomize() { /* Do nothing */ }

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		///  Gets the enumerator for the letters in the Scene ID.
		/// </summary>
		/// <returns>An enumerator to traverse the letters in the Scene ID.</returns>
		public IEnumerator<Letter> GetEnumerator() => ((IEnumerable<Letter>) Letters).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Letters.GetEnumerator();

		#endregion

		#region Comparison Operators

		public static bool operator ==(SceneId a, SceneId b) =>  a.Equals(b);
		public static bool operator !=(SceneId a, SceneId b) => !a.Equals(b);

		public static bool operator <(SceneId a, SceneId b) => a.CompareTo(b) < 0;
		public static bool operator >(SceneId a, SceneId b) => a.CompareTo(b) > 0;

		public static bool operator <=(SceneId a, SceneId b) => a.CompareTo(b) <= 0;
		public static bool operator >=(SceneId a, SceneId b) => a.CompareTo(b) >= 0;

		#endregion

		#region Casting

		public static implicit operator SceneId(PasswordSceneId pwid) => new SceneId(pwid);
		public static implicit operator SceneId(string s) => new SceneId(s);
		public static implicit operator SceneId(int v) => new SceneId(v);

		public static explicit operator string(SceneId id) => id.String;
		public static explicit operator int(SceneId id) => id.Value;

		#endregion

		#region Helpers

		/// <summary>
		///  Returns true if the string is a valid Scene ID letter string.
		/// </summary>
		/// <param name="s">The string to validate.</param>
		/// <returns>True if the string is a valid Scene ID letter string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValidString(string s) => Letter.IsValidString(s, Length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateLetters(Letter[] l, string paramName) {
			if (l == null)
				throw new ArgumentNullException(paramName);
			if (l.Length != Length)
				throw new ArgumentException($"Scene ID letters must be {Length} letters long, got {l.Length} letters!",
					paramName);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateString(ref string s, string paramName) {
			if (s == null)
				throw new ArgumentNullException(paramName);
			if (s.Length != Length)
				throw new ArgumentException($"Scene ID string must be {Length} letters long, got {s.Length} letters!",
					paramName);
			s = s.ToUpper();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateValue(int v, string paramName) {
			if (v < MinValue || v > MaxValue)
				throw new ArgumentOutOfRangeException(paramName, v,
					$"Scene ID value must be between {MinValue} and {MaxValue}, got {v}!");
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CopyFromLetters(Letter[] l) {
			int value = 0;
			for (int i = 0; i < Length; i++)
				value |= l[i].Value << (i * Letter.ShiftValue);
			return value & MaxValue; // Make sure to cutoff the last two bits of the last letter
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CopyFromString(string s) {
			int value = 0;
			for (int i = 0; i < Length; i++)
				value |= Letter.GetValueOfChar(s[i]) << (i * Letter.ShiftValue);
			return value & MaxValue; // Make sure to cutoff the last two bits of the last letter
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CopyFromValue(int v) {
			return v;
		}

		/*public static int LettersToValue(Letter[] letters, string paramName) {
			int value = letters[0].Value;
			if (letters.Length >= 2) {
				value |= letters[1].Value << 4;
				if (letters.Length >= 3)
					value |= (letters[2].Value & 0x3) << 8;
			}

			return value;
		}*/

		#endregion
	}
}
