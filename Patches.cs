using System.Collections.Generic;
using HarmonyLib;
using System.Reflection.Emit;

namespace CBC
{
    public static class Patches
    {
        [HarmonyPatch(typeof(FlightConsole))]
        class FlightConsolePatches
        {
            [HarmonyTranspiler]
            [HarmonyPatch("ExitFlightConsole")]
            static IEnumerable<CodeInstruction> ExitFlightConsoleTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                int index = -1;
                var codes = new List<CodeInstruction>(instructions);

                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldfld && codes[i + 1].opcode == OpCodes.Callvirt && codes[i + 2].opcode == OpCodes.Brtrue)
                    {
                        index = i - 1;
                        break;
                    }
                }
                if (index > -1)
                {
                    codes.RemoveRange(index, 3);
                    codes.Insert(index, CodeInstruction.Call(typeof(BottomCamStreamer), nameof(BottomCamStreamer.IsShipCameraOn)));
                }
                return codes;
            }

            [HarmonyPrefix]
            [HarmonyPatch("ExitLandingView")]
            static void ExitLandingViewTranspiler()
            {
                BottomCamStreamer.instance.renderOnScreen = false;
            }

            [HarmonyTranspiler]
            [HarmonyPatch("UpdateLandingMode")]
            static IEnumerable<CodeInstruction> UpdateLandingModeTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                List<int> indexesOfLandingCamSeq = new List<int>();

                int index1 = -1;// of _landingCam
                int index2 = -1;// of _playerCam

                var codes = new List<CodeInstruction>(instructions);

                //Achar o primeiro tipo de sequencia e dependedo do que ldfld está chamando, colcoar no index1 ou index2
                for (var i = 0; i < codes.Count; i++)
                {
                    if (index1 == -1 || index2 == -1)
                    {
                        if (codes[i].opcode == OpCodes.Ldarg_0 && codes[i + 1].opcode == OpCodes.Ldfld &&
                        codes[i + 3].opcode == OpCodes.Callvirt)
                        {
                            if (codes[i + 1].LoadsField(AccessTools.Field(typeof(FlightConsole), "_landingCam")))
                                index1 = i;

                            else if (codes[i + 1].LoadsField(AccessTools.Field(typeof(FlightConsole), "_playerCam")))
                                index2 = i;
                        }
                    }
                    else
                    {
                        if (codes[i].opcode == OpCodes.Ldarg_0 && codes[i + 1].opcode == OpCodes.Ldfld &&
                       codes[i + 2].opcode == OpCodes.Callvirt)
                        {
                            if (codes[i + 1].LoadsField(AccessTools.Field(typeof(FlightConsole), "_landingCam")))
                                indexesOfLandingCamSeq.Add(i);

                            else
                                i += 2;

                        }
                    }
                }
                if (index1 > -1 && index2 > -1 && indexesOfLandingCamSeq.Count > 0)
                {
                    for (int i = indexesOfLandingCamSeq.Count - 1; i > -1; i--)
                    {
                        codes.RemoveRange(indexesOfLandingCamSeq[i], 3);

                        codes.Insert(indexesOfLandingCamSeq[i], CodeInstruction.Call(typeof(BottomCamStreamer), nameof(BottomCamStreamer.IsShipCameraOn)));
                    }

                    codes.RemoveRange(index2, 4);

                    codes.RemoveRange(index1, 4);
                    codes.Insert(index1, CodeInstruction.Call(typeof(BottomCamStreamer), nameof(BottomCamStreamer.SetTrueRenderOnScreen)));
                }

                return codes;
            }

            [HarmonyTranspiler]
            [HarmonyPatch("Update")]
            static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                List<int> indexesOfLandingCamSeq = new List<int>();

                var codes = new List<CodeInstruction>(instructions);

                for (var i = 0; i < codes.Count; i++)
                {

                    if (codes[i].opcode == OpCodes.Ldarg_0 && codes[i + 1].opcode == OpCodes.Ldfld &&
                   codes[i + 2].opcode == OpCodes.Callvirt)
                    {
                        if (codes[i + 1].LoadsField(AccessTools.Field(typeof(FlightConsole), "_landingCam")))
                            indexesOfLandingCamSeq.Add(i);
                        else
                            i += 2;

                    }
                }
                if (indexesOfLandingCamSeq.Count > 0)
                {
                    for (int i = indexesOfLandingCamSeq.Count - 1; i > -1; i--)
                    {
                        codes.RemoveRange(indexesOfLandingCamSeq[i], 3);
                        codes.Insert(indexesOfLandingCamSeq[i], CodeInstruction.Call(typeof(BottomCamStreamer), nameof(BottomCamStreamer.IsShipCameraOn)));
                    }
                }
                return codes;
            }
        }
        }
}
