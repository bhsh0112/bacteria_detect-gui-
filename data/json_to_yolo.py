#批量json转yolo
import json
import os
from PIL import Image

def convert(img_size, box):
    x1_center = box[0] + (box[2]-box[0]) / 2.0
    y1_center = box[1] + (box[3]-box[1]) / 2.0
    
    w_1 = box[2] - box[0]
    h_1 = box[3] - box[1]
    
    x1_normal = x1_center / img_size[0]
    y1_normal = y1_center / img_size[1]
    
    w_1_normal = w_1 / img_size[0]
    y_1_normal = h_1 / img_size[1]
    
    return (x1_normal, y1_normal, w_1_normal, y_1_normal)


def decode_json(json_floder_path,image_folder_path, json_name,classes):
    classes = classes.split(',')
    txt_name = 'labels/' + json_name[0:-5] + '.txt'
    txt_file = open(txt_name, 'w')

    json_path = os.path.join(json_floder_path, json_name)
    image_path=os.path.join(image_floder_path,json_name[0:-5]+'.jpg')
    data = json.load(open(json_path, 'r', encoding='utf-8'))

    with Image.open(image_path) as img:
        img_w, img_h = img.size
    print(img_h,img_w)

    for i in data['labels']:
        label = i['class']
        label_index = classes.index(label)
        w=i['width']/img_w
        h=i['height']/img_h
        # x=i['x']/img_w
        # y=i['y']/img_h
        x=(i['x'])/img_w+w/2.0
        y=(i['y'])/img_h+h/2.0
        

        # bb = (x1, y1, x2, y2)
        bbox = (x,y,w,h)
        txt_file.write( str(label_index) + " " + " ".join([str(i) for i in bbox]) + '\n')


if __name__ == "__main__":

    json_floder_path = 'Annotations/'  #改成自己的json文件存储路径
    image_floder_path='images/'
    json_names = os.listdir(json_floder_path)
    classes = "S.aureus,C.albicans"
    for json_name in json_names:
        decode_json(json_floder_path,image_floder_path, json_name,classes)


