using CLIInterface.Component;
using CLIInterface.EGui;
using CLIInterface.Fbx;
using CLIInterface.Geometry;
using J2y.Danuri;


namespace J2y
{
    namespace Interface
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // IEventSystemHandler
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [IEventSystemHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IEventSystemHandler { }
        #endregion

        #region [IContainerHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IContainerHandler : IEventSystemHandler
        {
            Container Container { get; }
        }
        #endregion

        #region [IColliderHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IColliderHandler : IContainerHandler
        {
            BoundingBox Boundingbox { get; }
            bool UseCollider { get; set; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IUIColliderHandler : IColliderHandler
        {
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IWidgetColliderHandler : IUIColliderHandler
        {
            EGuiWidget EGuiWidget { get; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPanelColliderHandler : IUIColliderHandler
        {
            EGuiPanel EGuiPanel { get; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IFbxColliderHandler : IColliderHandler
        {
            Fbx Fbx { get; }
        }
        #endregion

        #region [ISelectHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface ISelectHandler : IEventSystemHandler
        {
            void OnSelect(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IUpdateSelectedHandler : IEventSystemHandler
        {
            void OnUpdateSelected(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IDeselectHandler : IEventSystemHandler
        {
            void OnDeselect(JPointer.EventSystem.Data data);
        }
        #endregion

        #region [IPointerHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPointerEnterHandler : IEventSystemHandler
        {
            void OnPointerEnter(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPointerExitHandler : IEventSystemHandler
        {
            void OnPointerExit(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPointerHandler : IEventSystemHandler
        {
            void OnPointer(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPointerDownHandler : IEventSystemHandler
        {
            void OnPointerDown(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPointerUpHandler : IEventSystemHandler
        {
            void OnPointerUp(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPointerDoubleClickHandler : IEventSystemHandler
        {
            void OnPointerDoubleClick(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IPointerClickHandler : IEventSystemHandler
        {
            void OnPointerPress(JPointer.EventSystem.Data data);
        }
        #endregion

        #region [IDragHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IInitializePotentialDragHandler : IEventSystemHandler
        {
            void OnInitializePotentialDrag(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IBeginDragHandler : IEventSystemHandler
        {
            void OnBeginDrag(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IDragHandler : IEventSystemHandler
        {
            void OnDrag(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IEndDragHandler : IEventSystemHandler
        {
            void OnEndDrag(JPointer.EventSystem.Data data);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IDropHandler : IEventSystemHandler
        {
            void OnDrop(JPointer.EventSystem.Data data);
        }
        #endregion

        #region [IScrollHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IScrollHandler : IEventSystemHandler
        {
            void OnScroll(JPointer.EventSystem.Data data);
        }
        #endregion

        #region [IMoveHandler]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public interface IMoveHandler : IEventSystemHandler
        {
            void OnMove();
        }
        #endregion

    }
}

