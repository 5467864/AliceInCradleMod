using UnityEngine;
using UnityEngine.EventSystems;

namespace AliceInCradle.Mono
{
    public class ModUIDrag : MonoBehaviour,IBeginDragHandler, IDragHandler ,IEndDragHandler
    {
        public static Vector3 Pos;
        private Vector3 _offset;             // UI和鼠标指针位置的偏移
        
        private float _minWidth;             // 水平最小拖拽范围
        private float _maxWidth ;            // 水平最大拖拽范围
        private float _minHeight;            // 垂直最小拖拽范围  
        private float _maxHeight;            // 垂直最大拖拽范围
        private float _rangeX;               // 水平拖拽范围
        private float _rangeY;               // 垂直拖拽范围
        
        private RectTransform _rt;

        public void Start()
        {
            _rt = transform.Find("BG").GetComponent<RectTransform>();
            Pos = ConfigManage.Pos.Value;
            if (!Pos.Equals(default))
            {
                _rt.position = Pos;
            }
            _minWidth = _rt.rect.width / 2;                           
            _maxWidth = Screen.width - (_rt.rect.width / 2);
            _minHeight = _rt.rect.height / 2;
            _maxHeight = Screen.height - (_rt.rect.height / 2);
        }
        private void Update()
        {
            DragRangeLimit();
        }
        private void OnDestroy()
        {
            ConfigManage.Pos.Value = Pos;
        }
        
        // 拖拽范围限制
        private void DragRangeLimit()
        {
            //限制水平/垂直拖拽范围在最小/最大值内
            _rangeX = Mathf.Clamp(_rt.position.x, _minWidth, _maxWidth);
            _rangeY = Mathf.Clamp(_rt.position.y, _minHeight, _maxHeight);
            //更新位置
            _rt.position = new Vector3(_rangeX, _rangeY, 0);
        }
        
        // 开始拖拽
        public void OnBeginDrag(PointerEventData eventData)
        {
            //将屏幕坐标转换成世界坐标
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rt, eventData.position, null, out var globalMousePos))
            {
                //计算UI和指针之间的位置偏移量
                _offset = _rt.position - globalMousePos;
            }
        }
        // 正在拖拽
        public void OnDrag(PointerEventData eventData)
        {
            SetDraggedPosition(eventData);
            // UImage.transform.position = eventData.position - _offset;
        }
        // 结束拖拽
        public void OnEndDrag(PointerEventData eventData)
        {
            Pos = _rt.position;
            // Plugin.EVENT.LogInfo(_rt.position);
        }

        // 更新位置
        private void SetDraggedPosition(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rt, eventData.position, null, out var globalMousePos))
            {
                _rt.position = _offset + globalMousePos;
            }
        }
    }
}