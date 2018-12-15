﻿/* 
 * This class contains the actual command packets for the LPMSB2
 * These are manually packed, due to issues with the default LPMSB2 class
 * constants 
 */

public static class LpmsCommands
{
    public static readonly byte[] resetOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x52, 0x00, 0x00, 0x00, 0x53, 0x00, 0x0D, 0x0A };
    public static readonly byte[] setObjectOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x12, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x13, 0x00, 0x0D, 0x0A };
    public static readonly byte[] setHeadingOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x12, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x0D, 0x0A };
    public static readonly byte[] setAlignmentOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x12, 0x00, 0x04, 0x00, 0x02, 0x00, 0x00, 0x00, 0x15, 0x00, 0x0D, 0x0A };
    public static readonly byte[] setCommandMode = new byte[] { 0x3A, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x07, 0x00, 0x0D, 0x0A };
    public static readonly byte[] setStreamMode = new byte[] { 0x3A, 0x01, 0x00, 0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x0D, 0x0A };
}
