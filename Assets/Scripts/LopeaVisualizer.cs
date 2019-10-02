using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LopeaVisualizer : MonoBehaviour
{
     
    [SerializeField] Vector3[,] positions;
    [SerializeField] int iterations = 0;
    
    [SerializeField] FFTSize quality = FFTSize.basic;
    [SerializeField] AudioSource Audio;
    [SerializeField] bool allowwaterfall;
    Vector3 currentScale = Vector3.one;
     
    float[] fftdata;
    // Start is called before the first frame update
    void Start()
    {
      generateinitpositions();                
    }
    
    // Update is called once per frame
    void Update()
    {
      shiftvectors();
      applyaudiodata();
      if(allowwaterfall)
        applywaterfall();
      
    }
    void applyaudiodata(){
      Audio.GetSpectrumData(fftdata, 0 , FFTWindow.BlackmanHarris);             
      normalize(fftdata);                                                       
      for(int x = 0; x < (int)quality; x++)                                     
      {                                                                         
        positions[x,0].y =                                                      
          fftdata[x];                                                           
                                                                                
      }      
    }
    void applywaterfall(){
      for(int y =0; y < iterations; y++){
        for(int x = 0; x < (int)quality; x++){
          positions[x,y] = Vector3.Scale(positions[x,y],transform.localScale);
        }
      }
    }
    void generateinitpositions(){
      fftdata = new float[(int)quality];
      positions = new Vector3[(int)quality, iterations];
      for(int y = 0; y < iterations; y++ ){
        for(int x = 0; x < (int)quality; x++){
          positions[x,y] = 
            new Vector3(x, 0, y);  
        }
      }
    }
    void shiftvectors(){
      for(int y = iterations - 2; y >= 0; y-- ){
        for(int x =0; x < (int)quality; x++){
          positions[x, y+1].y = positions[x,y].y;
        } 
      }       
    }
    void OnDrawGizmos(){
      if(positions != null){
        for(int y = 0; y < iterations; y++){
          for(int x = 0; x < (int) quality - 1; x++){
            Gizmos.DrawLine(positions[x,y], positions[x+1,y]);
          }
        }
      }
    }
  void normalize(float[] values){
    float mag = 0;
    foreach(float val in values ){
      mag += Mathf.Pow(val,2);
    }
    mag = Mathf.Sqrt(mag);
    for(int i = 0; i< values.Length; i ++){
      values[i] = mag != 0 ? values[i]/mag : 0;
    }
    
  }
}

public enum FFTSize{
  basic = 64,
  low = 128,
  medium = 256,
  high = 512,
  expert = 1024,
  expertplus = 2048,
  supercomputer = 4096
}
