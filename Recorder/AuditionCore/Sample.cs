using System;

namespace Recorder
{
	/// <summary>
	/// Sound data sample
	/// </summary>
	public class Sample
	{
		public float Value { get; private set; }
		public TimeSpan TimeSpan { get; private set; }

		public Sample(float value, TimeSpan timeSpan)
		{
			Value = value;
			TimeSpan = timeSpan;
		}
	}
}