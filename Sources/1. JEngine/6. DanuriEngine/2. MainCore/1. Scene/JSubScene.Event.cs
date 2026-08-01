namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSubScene.Evnet
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public abstract partial class JSubScene
    {
        public sealed class Evnet
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 이벤트
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [이벤트] Change
            public const string OnBegineChangeSubScene = "OnBegineChangeSubScene";
            public const string OnEndChangeSubScene = "OnEndChangeSubScene";
            #endregion

        }
    }
}

