using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlexus : MonoBehaviour
{
    public float maxDistance = 1f;
    public int maxConnections = 5;
    public int maxLineRenderers = 100;
    public float duration = 5f;


    new ParticleSystem particleSystem;


    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule particleSystemMainModule;


    public LineRenderer lineRendererTemplate;
    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    Transform _transform;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;

        
    }

    
    void LateUpdate()
    {
        int maxParticles = particleSystemMainModule.maxParticles;

        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        int lrIndex = 0;
        int lineRendererCount = lineRenderers.Count;

        if (lineRenderers.Count > maxLineRenderers)
        {
            for (int i = maxLineRenderers; i < lineRendererCount; i++)
            {
                /*float alpha = 1f;
                LeanTween.value(alpha, 0f, 0.3f).setOnUpdate((float t) =>
                            {
                                alpha = t;
                                lineRenderers[i].startColor = new Color(1f, 1f, 1f, alpha);
                                lineRenderers[i].endColor = new Color(1f, 1f, 1f, alpha);
                            }).setOnComplete(() =>
                            {*/
                            // LeanTween.color(lineRenderers[i].gameObject, new Color(1f,1f,1f,0f), 0.3f).setOnComplete(() =>
                            // {
                            //     Destroy(lineRenderers[i].gameObject);
                            // });
                            //});

                             Destroy(lineRenderers[i].gameObject);
            }
            int removedCount = lineRendererCount - maxLineRenderers;
            lineRenderers.RemoveRange(maxLineRenderers, removedCount);
            lineRendererCount -= removedCount;
        }


        if (maxConnections > 0 && maxLineRenderers > 0)
        {

            particleSystem.GetParticles(particles);
            int particleCount = particleSystem.particleCount;

            float maxDistanceSqr = maxDistance * maxDistance;

            switch(particleSystemMainModule.simulationSpace)
            {
                case ParticleSystemSimulationSpace.Local:
                {
                   _transform = transform;
                   lineRendererTemplate.useWorldSpace = false;
                   break;
                }
                case ParticleSystemSimulationSpace.Custom:
                {
                    _transform = particleSystemMainModule.customSimulationSpace;
                    lineRendererTemplate.useWorldSpace = false;
                    break;
                }
                case ParticleSystemSimulationSpace.World:
                {
                    _transform = transform;
                    lineRendererTemplate.useWorldSpace = true;
                    break;
                }
                default:
                {
                    throw new System.NotSupportedException(
                        string.Format("unsupported simulation space '{0)'.", 
                        System.Enum.GetName(typeof(ParticleSystemSimulationSpace), particleSystemMainModule.simulationSpace)));
                
                }
            }
            for (int i = 0; i< particleCount; i++)
            {
                Vector3 p1_position = particles[i].position;

                int connections = 0;

                for (int j = i + 1; j < particleCount; j++)
                {
                    if (lrIndex == maxLineRenderers)
                        {
                            break;
                        }

                    Vector3 p2_position = particles[j].position;
                    float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);

                    if (distanceSqr <= maxDistanceSqr)
                    {

                        LineRenderer lr;
                        if (lrIndex == lineRendererCount)
                        {
                            //var t = 0f;
                            lr = Instantiate(lineRendererTemplate, _transform, false);
                            LeanTween.color(lr.gameObject, new Color(1f,1f,1f,1f), 0.5f);
                            //var fos = 1f;
                            //fos = fos += Time.deltaTime;
                            /*Color m_color = Color.Lerp(new Color(0f,0f,0f,0f), new Color (1f, 1f, 1f, 1f), t);
                            lr.materials[0].SetColor("_TintColor", m_color);
                            if (t < 1) 
                            {                            
                                t += Time.deltaTime/duration;
                            }
                            /*float alpha = 0f;
                            LeanTween.value(alpha, 1f, 0.5f).setOnUpdate((float t) =>
                            {
                                alpha = t;
                                lr.startColor = new Color(1f, 1f, 1f, alpha);
                                lr.endColor = new Color(1f, 1f, 1f, alpha);
                            });*/

                            
                            lineRenderers.Add(lr);

                            lineRendererCount++;
                        }

                        lr = lineRenderers[lrIndex];

                        lr.enabled = true;

                        lr.SetPosition(0, p1_position);
                        lr.SetPosition(1, p2_position);

                        lrIndex++;
                        connections++;

                        if (connections == maxConnections || lrIndex == maxLineRenderers)
                        {
                            break;
                        }
                    }

                }
            }
        }

        for (int i = lrIndex; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].enabled = false;
        }
    }
}
