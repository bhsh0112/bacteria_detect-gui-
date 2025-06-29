from ultralytics import YOLO

# Load a COCO-pretrained YOLOv8n model
model = YOLO("yolov8n.pt")

# Display model information (optional)
model.info()

# Train the model on the COCO8 example dataset for 100 epochs
results = model.train(data="./data/jun.yaml", epochs=600, imgsz=4000,batch=2)

# Save the trained model
# model.export(format="pt")