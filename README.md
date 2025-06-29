# 功能概述
    一款具有用户界面的目标检测工具，能够实现对选中图像文件的目标检测及结果展示与保存
# 文件结构
- data:数据集
- ImageDetectionAPP：核心功能代码
  - detect.pu：python实现的目标检测功能代码
  - Program.cs：C#实现的用户界面
  - best.pt：目标检测所用权重
  - ……
- train：训练相关代码
  - train.py：训练脚本
  - labels_vis.py：标注文件可视化脚本，用于验证数据集有效性
- README.md：说明文档
# GetStart
