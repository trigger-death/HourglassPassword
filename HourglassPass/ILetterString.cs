﻿using System;
using System.Collections.Generic;

namespace HourglassPass {
	/// <summary>
	///  An interface for all password structures containing <see cref="Letter"/> arrays.
	/// </summary>
	public interface ILetterString : IReadOnlyLetterString {
		#region Properties

		/// <summary>
		///  Gets or sets the letter at the specified index in the letter string.
		/// </summary>
		/// <param name="index">The index of the letter.</param>
		/// <returns>The letter at the specified index in the letter string.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.
		/// </exception>
		new Letter this[int index] { get; set; }
		/// <summary>
		///  Gets or sets the letter string with an array of <see cref="Count"/> letters.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <see cref="Letters"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  The length of <see cref="Letters"/> is not <see cref="Count"/>.
		/// </exception>
		new Letter[] Letters { get; set; }
		/// <summary>
		///  Gets or sets the letter string with a string of <see cref="Count"/> letters.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <see cref="String"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  The length of <see cref="String"/> is not <see cref="Count"/>.-or- A character in <see cref="String"/> is
		///  not a valid letter character.
		/// </exception>
		new string String { get; set; }
		/// <summary>
		///  Gets or sets the letter string with a numeric value.
		/// </summary>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <see cref="Value"/> is less than <see cref="MinValue"/> or greater than <see cref="MaxValue"/>.
		/// </exception>
		new int Value { get; set; }

		#endregion

		#region Mutate

		/// <summary>
		///  Returns a letter string with normalized interchangeable characters.
		/// </summary>
		/// <param name="garbageChar">The character to use for garbage letters.</param>
		/// <returns>The normalized Password with consistent interchangeable characters.</returns>
		new ILetterString Normalized(char garbageChar = Letter.GarbageChar);
		/// <summary>
		///  Returns a letter string with randomized interchangeable characters.
		/// </summary>
		/// <returns>The randomized Password with random interchangable characters.</returns>
		new ILetterString Randomized();

		/// <summary>
		///  Normalizes the letter string's interchangeable characters.
		/// </summary>
		/// <param name="garbageChar">The character to use for garbage letters.</param>
		void Normalize(char garbageChar = Letter.GarbageChar);
		/// <summary>
		///  Randomizes the letter string's interchangeable characters.
		/// </summary>
		void Randomize();

		#endregion
	}
}
