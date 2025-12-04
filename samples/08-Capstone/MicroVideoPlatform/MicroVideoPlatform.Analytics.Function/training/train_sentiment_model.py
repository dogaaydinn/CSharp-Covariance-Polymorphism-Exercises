"""
ML Model Training Script for Sentiment Analysis
================================================

This script demonstrates how to train a sentiment analysis model
that can be used with ML.NET.

DEPENDENCIES:
pip install scikit-learn pandas numpy joblib

USAGE:
python train_sentiment_model.py

OUTPUT:
- sentiment_model.pkl (scikit-learn model)
- For ML.NET integration, export to ONNX format
"""

import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix
import joblib

# Sample training data (in production, load from database or CSV)
training_data = [
    # Positive examples
    ("This video is amazing! Great content!", 1),
    ("Love it! Very helpful tutorial.", 1),
    ("Excellent work, keep it up!", 1),
    ("Best video I've seen on this topic", 1),
    ("Thank you for sharing this!", 1),
    ("Very informative and well explained", 1),
    ("Great quality content, subscribed!", 1),
    ("This helped me a lot, thanks!", 1),
    ("Perfect! Exactly what I needed", 1),
    ("Outstanding video, loved it", 1),
    ("Brilliant explanation, thank you!", 1),
    ("This is exactly what I was looking for", 1),
    ("Awesome content, very useful", 1),
    ("Really appreciate this tutorial", 1),
    ("Fantastic work, well done!", 1),

    # Negative examples
    ("This is terrible, waste of time", 0),
    ("Boring content, don't recommend", 0),
    ("Not helpful at all", 0),
    ("Disliked, very poor quality", 0),
    ("This video is misleading", 0),
    ("Awful content, unsubscribed", 0),
    ("Worst tutorial ever", 0),
    ("Complete waste of my time", 0),
    ("Disappointed with this video", 0),
    ("Horrible, do not watch", 0),
    ("This is useless information", 0),
    ("Terrible quality, very bad", 0),
    ("Not worth watching at all", 0),
    ("I regret watching this", 0),
    ("Poorly made and unhelpful", 0),
]

# Create DataFrame
df = pd.DataFrame(training_data, columns=['text', 'label'])

# Split data
X_train, X_test, y_train, y_test = train_test_split(
    df['text'], df['label'], test_size=0.2, random_state=42
)

print(f"Training samples: {len(X_train)}")
print(f"Test samples: {len(X_test)}")

# Feature extraction (TF-IDF)
vectorizer = TfidfVectorizer(
    max_features=1000,
    min_df=1,
    max_df=0.8,
    ngram_range=(1, 2)
)

X_train_tfidf = vectorizer.fit_transform(X_train)
X_test_tfidf = vectorizer.transform(X_test)

print(f"\nFeature vector shape: {X_train_tfidf.shape}")

# Train model (Logistic Regression)
model = LogisticRegression(
    C=1.0,
    max_iter=1000,
    random_state=42
)

model.fit(X_train_tfidf, y_train)

# Evaluate
y_pred = model.predict(X_test_tfidf)
accuracy = accuracy_score(y_test, y_pred)

print(f"\nModel Accuracy: {accuracy:.2%}")
print("\nClassification Report:")
print(classification_report(y_test, y_pred, target_names=['Negative', 'Positive']))

print("\nConfusion Matrix:")
print(confusion_matrix(y_test, y_pred))

# Save model
joblib.dump(model, 'sentiment_model.pkl')
joblib.dump(vectorizer, 'sentiment_vectorizer.pkl')

print("\nModel saved to sentiment_model.pkl")
print("Vectorizer saved to sentiment_vectorizer.pkl")

# Test predictions
test_comments = [
    "This is great!",
    "This is terrible!",
    "Amazing tutorial, thanks!",
    "Waste of time, very bad",
]

print("\n--- Test Predictions ---")
for comment in test_comments:
    features = vectorizer.transform([comment])
    prediction = model.predict(features)[0]
    probability = model.predict_proba(features)[0]

    sentiment = "Positive" if prediction == 1 else "Negative"
    confidence = probability[prediction] * 100

    print(f"Comment: '{comment}'")
    print(f"  Sentiment: {sentiment} (Confidence: {confidence:.1f}%)\n")

# Export to ONNX (for ML.NET integration)
try:
    from skl2onnx import convert_sklearn
    from skl2onnx.common.data_types import FloatTensorType

    # Define input type
    initial_type = [('float_input', FloatTensorType([None, X_train_tfidf.shape[1]]))]

    # Convert to ONNX
    onnx_model = convert_sklearn(model, initial_types=initial_type)

    # Save ONNX model
    with open("sentiment_model.onnx", "wb") as f:
        f.write(onnx_model.SerializeToString())

    print("ONNX model saved to sentiment_model.onnx")
    print("This model can be used with ML.NET TensorFlow integration")
except ImportError:
    print("\nNote: skl2onnx not installed. To export ONNX model, run:")
    print("pip install skl2onnx onnxmltools")
