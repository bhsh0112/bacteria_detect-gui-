#!/usr/bin/env python3
import argparse
import json
import os
import sys
from pathlib import Path
from ultralytics import YOLO

def detect_image(image_path, conf,model_path, output_dir):
    # 确保输出目录存在
    
    Path(output_dir).mkdir(parents=True, exist_ok=True)
    
    # 加载模型
    model = YOLO(model_path)
    
    # 进行预测
    results = model(image_path)
    
    
    # 保存结果图像
    output_path = os.path.join(output_dir, f"detected_{os.path.basename(image_path)}")
    
    results[0].save(filename=output_path)
    # print("success!!!!!!!!!!!!!!!!!!!!!!!")
    
    # 准备检测结果数据
    detections = []
    for result in results:
        for box in result.boxes:
            conf = float(box.conf[0])
            if conf < conf:  # 应用置信度阈值
                continue
            cls = int(box.cls[0])
            conf = float(box.conf[0])
            x1, y1, x2, y2 = map(float, box.xyxy[0])
            
            detections.append({
                "class": result.names[cls],
                "confidence": conf,
                "bbox": [x1, y1, x2, y2]
            })
    
    # 返回结果图像路径和检测数据
    return {
        "output_image": output_path,
        "detections": detections
    }

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="YOLO Object Detection")
    parser.add_argument("--image", type=str, required=True, help="Path to input image")
    parser.add_argument("--model", type=str, default="best.pt", help="Path to model weights")
    parser.add_argument("--output", type=str, default="output", help="Output directory")
    parser.add_argument("--conf", type=float, default=0.25, help="Confidence threshold")
    
    args = parser.parse_args()
    
    try:
        result = detect_image(args.image, args.conf,args.model, args.output)
        # 输出JSON格式的结果
        print(json.dumps(result))
    except Exception as e:
        print(json.dumps({"error": str(e)}))
        sys.exit(1)