using UnityEngine;


public class Visualizer : MonoBehaviour
{
	public GameObject CubePrefab;					
	public int NumberOfCubes = 20;
	public float CubeCircleRadius = 5f;
	public GameObject[] Cubes;
	public float Speed = 1f;
	public Transform TransformParent;
	public float CubeEmphasis = 100f;

	public float[] SpectrumDataDebug;

	private float[] freqData;
	private int numSamples = 1024;
	private float maxFreq;

	void Start()
	{
		// Instaniate Cubes in a Circle around center 
		Cubes = new GameObject[NumberOfCubes];
		for (var i = 0; i < NumberOfCubes; i++)
		{
			var angle = i*Mathf.PI*2/NumberOfCubes;
			var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle))*CubeCircleRadius;
			Cubes[i] =
				Instantiate(CubePrefab, pos, Quaternion.identity) as GameObject;
			Cubes[i].transform.SetParent(TransformParent, true);
		}

		// Setup Variables for Frequency 
		freqData = new float[numSamples];
		maxFreq = AudioSettings.outputSampleRate / 2;
	}

	void Update()
	{
		var spectrum = AudioListener.GetSpectrumData(1024, 0,
			FFTWindow.BlackmanHarris);
		freqData = spectrum;

		for (var cubeIndex = 0; cubeIndex < NumberOfCubes; cubeIndex++)
		{
			var scale = Cubes[cubeIndex].transform.localScale;
			scale.y = Mathf.Lerp(scale.y, spectrum[cubeIndex] * CubeEmphasis,
				Time.deltaTime * Speed);
		    var position = Cubes[cubeIndex].transform.localPosition;
            position.y = scale.y/2 + 1.1f;
			Cubes[cubeIndex].transform.localScale = scale;
			Cubes[cubeIndex].transform.localPosition = position;
		}

		Debug.Log("Bass: " + GetBass() + " | Treble: " + GetTreble());

	}

	/**
	 * Gets the Average Volume of a given Frequency Band 
	 */ 
	float BandVol (float lowFreq, float highFreq)
	{
		// Confine High and Low End of Frequencies Specturm 
		lowFreq = Mathf.Clamp (lowFreq, 20, maxFreq); // Limit Low Frequencies 
		highFreq = Mathf.Clamp(highFreq, lowFreq, maxFreq);

		int n1 = Mathf.FloorToInt (lowFreq * numSamples / maxFreq);
		int n2 = Mathf.FloorToInt (highFreq * numSamples / maxFreq);
		float sum = 0;

		// Average Volumes of Frequences lowFreq to highFreq
		for (var freqIndex = n1; freqIndex <= n2; freqIndex++) {
			sum += freqData [freqIndex];
		}

		return sum / (n2 - n1 + 1);
	}

	/**
	 * Gets the Volume of the Bass in the current Audio Sample
	 */ 
	float GetBass () {
		return BandVol (16, 256);
	}

	/**
	 * Gets the Volume of the Treble of the current Audio Sample 
	 */
	float GetTreble() {
		return BandVol (2048, 16384);
	}


}