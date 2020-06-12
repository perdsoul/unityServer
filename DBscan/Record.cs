using UnityEngine;
 namespace Resources.Scripts.DBscan
{
    public class Record:DatasetItemBase
    {
        public double posX;
        public double posY;
        public double posZ;

        public double rotX;
        public double rotY;
        public double rotZ;

        public Record(Vector3 position,Vector3 rotation)
        {
            posX = position.x;
            posY = position.y;
            posZ = position.z;
            rotX = rotation.x;
            rotY = rotation.y;
            rotZ = rotation.z;
        }
        public Record()
        {
            posX = new float();
            posY = new float();
            posZ = new float();
            rotX = new float();
            rotY = new float();
            rotZ = new float();
        }
    }
}
