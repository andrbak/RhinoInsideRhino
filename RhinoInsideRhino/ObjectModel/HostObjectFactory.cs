using Rhino.Geometry;

namespace RhinoInsideRhino.ObjectModel
{
    public static class HostObjectFactory
    {


        public static IHostObject CreateCustomObject(IHostUserData userData, GeometryBase geometry)
        {

            if (userData is  CurveHostUserData curveHostUserData)
            {
                return new CurveHostObject((Curve)geometry, curveHostUserData);
            }


            return null;

        }

    }

    }

