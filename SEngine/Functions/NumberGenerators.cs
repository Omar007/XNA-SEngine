using System.Security.Cryptography;

namespace SEngine.Functions
{
	public static class NumberGenerators
	{
		/// <summary>
		/// Creates an array of random bytes using 'RNGCryptoServiceProvider'.
		/// </summary>
		/// <param name="amount">Length of the array (amount of bytes to generate).</param>
		/// <returns>Byte array containing 'amount' bytes.</returns>
		public static byte[] getRandomBytes(uint amount)
		{
			byte[] bytes = new byte[amount];
			new RNGCryptoServiceProvider().GetBytes(bytes);

			return bytes;
		}

		/// <summary>
		/// Creates a random byte. Calls <see cref="getRandomBytes"/> using 'amount = 1'.
		/// </summary>
		/// <returns>A random byte.</returns>
		public static byte getRandomByte()
		{
			return getRandomBytes(1)[0];
		}
	}
}
