namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // [class]
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [class] UniqueID
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static class JUniqueID
    {
        // FixedID
        public const long EngineRoot = 1;
        public const long SceneManager = 2;
        public const long UserManager = 3;

        // Type
        public const long TypeMask = unchecked((long)0xffffffff00000000);
        public const long IDMask = unchecked(0x00000000ffffffff);
        public const int Type_General = 0;
        public const int Type_User = 1;
        public const int Type_Pawn = 2;
        public const int Type_Field = 3;


        #region [생성] ID
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long NewID()
        {
            return JUtil.CreateUniqueId();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long MakeID(int type, int id)
        {
            return ((long)type << 32) | (long)id;
        }
        #endregion


        #region [Type] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int GetType(long id)
        {
            return (int)((TypeMask & id) >> 32);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int GetID(long id)
        {
            return (int)(IDMask & id);
        }
        #endregion
    }
    #endregion

}
