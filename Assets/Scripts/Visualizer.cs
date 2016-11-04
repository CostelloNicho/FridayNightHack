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
	private float[] numSamples = 1024;
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
			FFTWindow.Hamming);
		SpectrumDataDebug = spectrum;

		for (var cubeIndex = 0; cubeIndex < NumberOfCubes; cubeIndex++)
		{
			var scale = Cubes[cubeIndex].transform.localScale;
			scale.y = Mathf.Lerp(scale.y, spectrum[cubeIndex] * CubeEmphasis,
				Time.deltaTime * Speed);
			Cubes[cubeIndex].transform.localScale = scale;
		}
	}

	void BandVol (float lowFreq, float highFreq)
	{
		lowFreq = Mathf.Clamp (lowFreq, 20, maxFreq); // Limit Low Frequencies 
		highFreq = Mathf.Clamp(highFreq, lowFreq, maxFreq);

	}
}