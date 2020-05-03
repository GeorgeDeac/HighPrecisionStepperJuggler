﻿
using UnityEngine;

namespace HighPrecisionStepperJuggler.MachineLearning
{
    // This class implements Gradient Descent as explained by Andrew Ng in this video:
    // https://www.youtube.com/watch?v=5u4G23_OohI
    public class GradientDescent
    {
        public TrainingSet[] TrainingSets => _trainingSets;
        private TrainingSet[] _trainingSets;

        public Hypothesis Hypothesis => _hypothesis;
        private Hypothesis _hypothesis;

        public GradientDescent()
        {
            // NOTE: Newest training set at index 0
            _trainingSets = new TrainingSet[Constants.MaxNumberOfTrainingSets];
            _hypothesis = new Hypothesis(new Parameters(0f, 0f));

            for (int i = 0; i < Constants.MaxNumberOfTrainingSets; i++)
            {
                _trainingSets[i] = new TrainingSet(0f, 0f);
            }
        }

        public void AddTrainingSet(TrainingSet set)
        {
            for (int i = _trainingSets.Length - 1; i > 0; i--)
            {
                _trainingSets[i - 1].t_1 -= Time.deltaTime;
                _trainingSets[i] = _trainingSets[i - 1];
            }

            _trainingSets[0] = set;
        }

        public void UpdateHypothesis()
        {
            for (int i = 0; i < 500; i++)
            {
                _hypothesis.Update(_trainingSets);
            }
        }
    }
}

