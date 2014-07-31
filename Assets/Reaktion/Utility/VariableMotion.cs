﻿//
// Reaktion - An audio reactive animation toolkit for Unity.
//
// Copyright (C) 2013, 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;

namespace Reaktion {

public class VariableMotion : MonoBehaviour
{
    // Options for transfomation.
    public enum TransformMode {
        Off, XAxis, YAxis, ZAxis, Arbitral, Random
    };

    // A class for handling each transformation.
    [System.Serializable]
    public class TransformElement
    {
        public TransformMode mode = TransformMode.Off;
        public AnimationCurve curve = AnimationCurve.Linear(0, -1, 1, 1);
        public float amplitude = 1;
        public float timeScale = 1;

        // Used only in the arbitral mode.
        public Vector3 arbitralVector = Vector3.up;

        // Affects amplitude and time scale.
        public float randomness = 0;

        // Randomizer states.
        Vector3 randomVector;
        float randomAmplitude;
        float randomTimeScale;

        // Time parameter.
        float time;

        public void Initialize()
        {
            randomVector = Random.onUnitSphere;
            randomAmplitude = Random.value;
            randomTimeScale = Random.value;
            time = 0;
        }

        // Get a vector corresponds to the current transform mode.
        public Vector3 Vector {
            get {
                switch (mode)
                {
                    case TransformMode.XAxis:    return Vector3.right;
                    case TransformMode.YAxis:    return Vector3.up;
                    case TransformMode.ZAxis:    return Vector3.forward;
                    case TransformMode.Arbitral: return arbitralVector;
                    case TransformMode.Random:   return randomVector;
                }
                return Vector3.zero;
            }
        }

        // Advance the time parameter.
        public void Step()
        {
            time += Time.deltaTime * (1.0f - randomTimeScale * randomness);
        }

        // Get the current scalar value.
        public float Scalar {
            get {
                var amp = amplitude * (1.0f - randomAmplitude * randomness);
                return curve.Evaluate(time) * amp;
            }
        }
    }

    public TransformElement position = new TransformElement();
    public TransformElement rotation = new TransformElement();
    public bool useLocalCoordinate = true;

    void OnEnable()
    {
        position.Initialize();
        rotation.Initialize();
    }

    void Update()
    {
        if (position.mode != TransformMode.Off)
        {
            position.Step();
            if (useLocalCoordinate)
                transform.localPosition = position.Vector * position.Scalar;
            else
                transform.position = position.Vector * position.Scalar;
        }

        if (position.mode != TransformMode.Off)
        {
            rotation.Step();
            var r = Quaternion.AngleAxis(rotation.Scalar, rotation.Vector);
            if (useLocalCoordinate)
                transform.localRotation = r;
            else
                transform.rotation = r;
        }
    }
}

} // namespace Reaktion
