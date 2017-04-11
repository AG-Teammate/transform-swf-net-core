namespace com.flagstone.transform.coder
{
	/// <summary>
	/// Coder contains constants and utility functions used by the various classes
	/// for encoding and decoding.
	/// </summary>
	public sealed class Coder
	{
		/// <summary>
		/// Mask for getting and setting bit 0 of a word. </summary>
		public const int BIT0 = 0x00000001;
		/// <summary>
		/// Mask for getting and setting bit 1 of a word. </summary>
		public const int BIT1 = 0x00000002;
		/// <summary>
		/// Mask for getting and setting bit 2 of a word. </summary>
		public const int BIT2 = 0x00000004;
		/// <summary>
		/// Mask for getting and setting bit 3 of a word. </summary>
		public const int BIT3 = 0x00000008;
		/// <summary>
		/// Mask for getting and setting bit 4 of a word. </summary>
		public const int BIT4 = 0x00000010;
		/// <summary>
		/// Mask for getting and setting bit 5 of a word. </summary>
		public const int BIT5 = 0x00000020;
		/// <summary>
		/// Mask for getting and setting bit 6 of a word. </summary>
		public const int BIT6 = 0x00000040;
		/// <summary>
		/// Mask for getting and setting bit 7 of a word. </summary>
		public const int BIT7 = 0x00000080;
		/// <summary>
		/// Mask for getting and setting bit 10 of a word. </summary>
		public const int BIT10 = 0x00000400;
		/// <summary>
		/// Mask for getting and setting bit 15 of a word. </summary>
		public const int BIT15 = 0x00008000;

		/// <summary>
		/// Mask for accessing bits 0-3 of a word. </summary>
		public const int NIB0 = 0x0000000F;
		/// <summary>
		/// Mask for accessing bits 4-7 of a word. </summary>
		public const int NIB1 = 0x000000F0;
		/// <summary>
		/// Mask for accessing bits 8-11 of a word. </summary>
		public const int NIB2 = 0x00000F00;
		/// <summary>
		/// Mask for accessing bits 12-15 of a word. </summary>
		public const int NIB3 = 0x0000F000;

		/// <summary>
		/// Mask for accessing bits 0 & 1 of a byte. </summary>
		public const int PAIR0 = 0x0003;
		/// <summary>
		/// Mask for accessing bits 2 & 3 of a byte. </summary>
		public const int PAIR1 = 0x000C;
		/// <summary>
		/// Mask for accessing bits 0 & 1 of a byte. </summary>
		public const int PAIR2 = 0x0030;
		/// <summary>
		/// Mask for accessing bits 2 & 3 of a byte. </summary>
		public const int PAIR3 = 0x00C0;

		/// <summary>
		/// Bit mask for the lowest 5 bits in a word. </summary>
		public const int LOWEST3 = 0x0007;
		/// <summary>
		/// Bit mask for the lowest 5 bits in a word. </summary>
		public const int LOWEST5 = 0x001F;
		/// <summary>
		/// Bit mask for the lowest 5 bits in a word. </summary>
		public const int LOWEST7 = 0x007F;
		/// <summary>
		/// Bit mask for the lowest 5 bits in a word. </summary>
		public const int LOWEST10 = 0x03FF;
		/// <summary>
		/// Bit mask for the lowest 5 bits in a word. </summary>
		public const int LOWEST12 = 0x0FFF;
		/// <summary>
		/// Bit mask for the lowest 5 bits in a word. </summary>
		public const int LOWEST15 = 0x7FFF;

		/// <summary>
		/// Right shift to move upper byte of 16-bit word to lower. </summary>
		public const int TO_LOWER_BYTE = 8;
		/// <summary>
		/// Left shift to move lower byte of 16-bit word to upper. </summary>
		public const int TO_UPPER_BYTE = 8;

		/// <summary>
		/// Right shift to move upper byte of 16-bit word to lower. </summary>
		public const int TO_LOWER_NIB = 4;
		/// <summary>
		/// Left shift to move lower byte of 16-bit word to upper. </summary>
		public const int TO_UPPER_NIB = 4;

		/// <summary>
		/// Maximum value that can be stored in a 16-bit unsigned field. </summary>
		public const int USHORT_MAX = 65535;
		/// <summary>
		/// Minimum value that can be stored in a 16-bit signed field. </summary>
		public const int SHORT_MIN = -32768;
		/// <summary>
		/// Maximum value that can be stored in a 16-bit signed field. </summary>
		public const int SHORT_MAX = 32767;

		/// <summary>
		/// Number of bits to shift when aligning a value to the second byte. </summary>
		public const int ALIGN_BYTE1 = 8;
		/// <summary>
		/// Number of bits to shift when aligning a value to the third byte. </summary>
		public const int ALIGN_BYTE2 = 16;
		/// <summary>
		/// Number of bits to shift when aligning a value to the fourth byte. </summary>
		public const int ALIGN_BYTE3 = 24;

		/// <summary>
		/// Number of bits to shift when aligning bits 4-7 to positions 0-3. </summary>
		public const int ALIGN_NIB1 = 4;
		/// <summary>
		/// Number of bits to shift when aligning bits 8-11 to positions 0-3. </summary>
		public const int ALIGN_NIB2 = 8;
		/// <summary>
		/// Number of bits to shift when aligning bits 12-15 to positions 0-3. </summary>
		public const int ALIGN_NIB3 = 12;

		/// <summary>
		/// Factor for converting floats to/from 8.8 fixed-point values. </summary>
		public const float SCALE_8 = 256.0f;
		/// <summary>
		/// Factor for converting floats to/from 15.15 fixed-point values. </summary>
		public const float SCALE_14 = 16384.0f;
		/// <summary>
		/// Factor for converting floats to/from 16.16 fixed-point values. </summary>
		public const float SCALE_16 = 65536.0f;

		/// <summary>
		/// The maximum value for each byte in a variable length integer. </summary>
		public const int VAR_INT_MAX = 127;
		/// <summary>
		/// Shift when converting to a variable length integer. </summary>
		public const int VAR_INT_SHIFT = 7;

		/// <summary>
		/// Bit mask for extracting the length field from the header word.
		/// </summary>
		public const int LENGTH_FIELD = 0x3F;
		/// <summary>
		/// The number of bits used to encode the length field when the length is
		/// less than the maximum length of 62.
		/// </summary>
		public const int LENGTH_FIELD_SIZE = 6;
		/// <summary>
		/// Value used to indicate that the length of an object has been encoded
		/// as a 32-bit integer following the header for the MovieTag.
		/// </summary>
		public const int IS_EXTENDED = 63;
		/// <summary>
		/// Number of bytes occupied by the header when the size of the encoded
		/// object is 62 bytes or less.
		/// </summary>
		public const int SHORT_HEADER = 2;
		/// <summary>
		/// The maximum length in bytes of an encoded object before the length must
		/// be encoded using a 32-bit integer.
		/// </summary>
		public const int HEADER_LIMIT = 62;
		/// <summary>
		/// Number of bytes occupied by the header when the size of the encoded
		/// object is greater than 62 bytes.
		/// </summary>
		public const int LONG_HEADER = 6;
		/// <summary>
		/// Length, in bytes, of type and length fields of an encoded action.
		/// </summary>
		public const int ACTION_HEADER = 3;


		/// <summary>
		/// Number of bits in an int. </summary>
		private const int BITS_PER_INT = 32;
		/// <summary>
		/// Bit mask with most significant bit of a 32-bit integer set. </summary>
		private const int MSB_MASK = unchecked((int)0x80000000);

		/// <summary>
		/// Calculates the minimum number of bits required to encoded an unsigned
		/// integer in a bit field.
		/// </summary>
		/// <param name="value">
		///            the unsigned value to be encoded.
		/// </param>
		/// <returns> the number of bits required to encode the value. </returns>


		public static int unsignedSize(int value)
		{



			int val = (value < 0) ? -value - 1 : value;
			int counter = BITS_PER_INT;
			int mask = MSB_MASK;

			while (((val & mask) == 0) && (counter > 0))
			{
				mask = (int)((uint)mask >> 1);
				counter -= 1;
			}
			return counter;
		}

		/// <summary>
		/// Calculates the minimum number of bits required to encoded a signed
		/// integer in a bit field.
		/// </summary>
		/// <param name="value">
		///            the signed value to be encoded.
		/// </param>
		/// <returns> the number of bits required to encode the value. </returns>


		public static int size(int value)
		{
			int counter = BITS_PER_INT;
			int mask = MSB_MASK;


			int val = (value < 0) ? -value - 1 : value;

			while (((val & mask) == 0) && (counter > 0))
			{
				mask = (int)((uint)mask >> 1);
				counter -= 1;
			}
			return counter + 1;
		}

		/// <summary>
		/// Returns the minimum number of bits required to encode all the signed
		/// values in an array as a set of bit fields with the same size.
		/// </summary>
		/// <param name="values">
		///            an array of signed integers.
		/// </param>
		/// <returns> the minimum number of bits required to encode each of the values. </returns>


		public static int maxSize(params int[] values)
		{

			int max = 0;
			int sizeI;

			foreach (int value in values)
			{
				sizeI = size(value);
				max = (max > sizeI) ? max : sizeI;
			}
			return max;
		}

		/// <summary>
		/// Calculate minimum number of bytes a 32-bit unsigned integer can be
		/// encoded in.
		/// </summary>
		/// <param name="value">
		///            an integer containing the value to be written. </param>
		/// <returns> the number of bytes required to encode the integer. </returns>


		public static int sizeVariableU32(int value)
		{

			int val = value;
			int size = 1;

			while (val > VAR_INT_MAX)
			{
				size += 1;
				val = (int)((uint)val >> VAR_INT_SHIFT);
			}
			return size;
		}

		/// <summary>
		/// Private constructor. </summary>
		private Coder()
		{
			// Private Constructor
		}
	}

}