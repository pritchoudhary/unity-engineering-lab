using UnityEngine;
namespace EngineeringLab
{
    public class SpringDemo : MonoBehaviour
    {
        [Range(.25f,5)] public float mass=1;
        [Range(1,100)] public float stiffness=20;
        [Range(0,50)] public float damping=2;
        [Range(-2,2)] public float displacement=1.5f;
        private double x=1.5, v, elapsed, accumulator;
        private bool running;
        private void Update()
        {
            if (!running) return;
            accumulator += System.Math.Min(Time.unscaledDeltaTime,.05);
            while(accumulator>=SpringModel.StepSize)
            {
                SpringModel.Step(ref x,ref v,mass,stiffness,damping);
                accumulator-=SpringModel.StepSize; elapsed+=SpringModel.StepSize;
            }
        }
        private void ResetState(){x=displacement;v=0;elapsed=0;accumulator=0;}
        private void OnGUI()
        {
            var scale=Mathf.Min(Screen.width/900f,Screen.height/650f);
            GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(scale,scale,1));
            GUI.Box(new Rect(15,15,870,620),"SPRING PHYSICS LAB / UNITY");
            GUILayout.BeginArea(new Rect(40,55,820,560));
            GUILayout.Label("Mass–spring–damper • mẍ + cẋ + kx = 0");
            GUILayout.Label($"Displacement: {x:F3} m   Velocity: {v:F3} m/s   Energy: {SpringModel.Energy(x,v,mass,stiffness):F3} J   Time: {elapsed:F1} s");
            GUILayout.Space(160);
            GUI.changed=false;
            GUILayout.Label($"Mass: {mass:F2} kg"); mass=GUILayout.HorizontalSlider(mass,.25f,5);
            GUILayout.Label($"Stiffness: {stiffness:F1} N/m"); stiffness=GUILayout.HorizontalSlider(stiffness,1,100);
            GUILayout.Label($"Damping: {damping:F2} N·s/m"); damping=GUILayout.HorizontalSlider(damping,0,50);
            GUILayout.Label($"Initial displacement: {displacement:F2} m"); displacement=GUILayout.HorizontalSlider(displacement,-2,2);
            if(GUI.changed) ResetState();
            GUILayout.Space(15);GUILayout.BeginHorizontal();
            if(GUILayout.Button(running?"Pause":"Run"))running=!running;
            if(GUILayout.Button("Reset"))ResetState();
            if(GUILayout.Button("Critical damping")){damping=Mathf.Min(50,2*Mathf.Sqrt(mass*stiffness));ResetState();}
            GUILayout.EndHorizontal();
            GUILayout.Label("Fixed 240 Hz semi-implicit Euler. Long frames capped at 50 ms.");
            GUILayout.EndArea();
            var old=GUI.color; GUI.color=new Color(.85f,.3f,.12f);
            var bx=450+(float)x*100;
            GUI.DrawTexture(new Rect(80,155,bx-80,3),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(bx-20,137,40,40),Texture2D.whiteTexture);
            GUI.color=Color.gray;GUI.DrawTexture(new Rect(450,115,1,90),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(75,125,5,65),Texture2D.whiteTexture);GUI.color=old;
        }
    }
}
