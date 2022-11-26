using UnityEngine;

namespace InteligenciaArtificial.SegundoParcial.Agents
{
    public enum MOVE_DIRECTIONS
    {
        UP,
        LEFT,
        RIGHT,
        DOWN,
        NONE
    }

    public class AgentBehaviour : MonoBehaviour
    {
        #region PUBLIC_METHODS
        public void MoveOnDirection(MOVE_DIRECTIONS moveDirection, int limitX, int limitY)
        {
            switch (moveDirection)
            {
                case MOVE_DIRECTIONS.UP:

                    if(transform.position.y +1 < limitY)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    }

                    break;
                case MOVE_DIRECTIONS.LEFT:

                    if (transform.position.x - 1 > 0)
                    {
                        transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(limitY-1, transform.position.y, transform.position.z);
                    }

                    break;
                case MOVE_DIRECTIONS.RIGHT:

                    if(transform.position.x + 1 < limitY)
                    {
                        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(0, transform.position.y, transform.position.z);
                    }

                    break;
                case MOVE_DIRECTIONS.DOWN:

                    if(transform.position.y -1 > 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                    }

                    break;
                case MOVE_DIRECTIONS.NONE:
                    transform.position = transform.position;
                    break;
            }
        }
        #endregion
    }
}