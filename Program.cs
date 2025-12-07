using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using System.Buffers.Binary;

namespace GXTConvert
{
    public enum SceGxmTextureBaseFormat : uint
    {
        U8 = 0x00000000,
        S8 = 0x01000000,
        U4U4U4U4 = 0x02000000,
        U8U3U3U2 = 0x03000000,
        U1U5U5U5 = 0x04000000,
        U5U6U5 = 0x05000000,
        S5S5U6 = 0x06000000,
        U8U8 = 0x07000000,
        S8S8 = 0x08000000,
        U16 = 0x09000000,
        S16 = 0x0A000000,
        F16 = 0x0B000000,
        U8U8U8U8 = 0x0C000000,
        S8S8S8S8 = 0x0D000000,
        U2U10U10U10 = 0x0E000000,
        U16U16 = 0x0F000000,
        S16S16 = 0x10000000,
        F16F16 = 0x11000000,
        F32 = 0x12000000,
        F32M = 0x13000000,
        X8S8S8U8 = 0x14000000,
        X8U24 = 0x15000000,
        U32 = 0x17000000,
        S32 = 0x18000000,
        SE5M9M9M9 = 0x19000000,
        F11F11F10 = 0x1A000000,
        F16F16F16F16 = 0x1B000000,
        U16U16U16U16 = 0x1C000000,
        S16S16S16S16 = 0x1D000000,
        F32F32 = 0x1E000000,
        U32U32 = 0x1F000000,
        PVRT2BPP = 0x80000000,
        PVRT4BPP = 0x81000000,
        PVRTII2BPP = 0x82000000,
        PVRTII4BPP = 0x83000000,
        UBC1 = 0x85000000,
        UBC2 = 0x86000000,
        UBC3 = 0x87000000,
        YUV420P2 = 0x90000000,
        YUV420P3 = 0x91000000,
        YUV422 = 0x92000000,
        P4 = 0x94000000,
        P8 = 0x95000000,
        U8U8U8 = 0x98000000,
        S8S8S8 = 0x99000000,
        U2F10F10F10 = 0x9A000000
    };

    public enum SceGxmTextureSwizzle4Mode : ushort
    {
        ABGR = 0x0000,
        ARGB = 0x1000,
        RGBA = 0x2000,
        BGRA = 0x3000,
        _1BGR = 0x4000,
        _1RGB = 0x5000,
        RGB1 = 0x6000,
        BGR1 = 0x7000
    };

    public enum SceGxmTextureSwizzle3Mode : ushort
    {
        BGR = 0x0000,
        RGB = 0x1000
    };

    public enum SceGxmTextureSwizzle2Mode : ushort
    {
        GR = 0x0000,
        _00GR = 0x1000,
        GRRR = 0x2000,
        RGGG = 0x3000,
        GRGR = 0x4000,
        _00RG = 0x5000
    };

    public enum SceGxmTextureSwizzle2ModeAlt : ushort
    {
        SD = 0x0000,
        DS = 0x1000
    };

    public enum SceGxmTextureSwizzle1Mode : ushort
    {
        R = 0x0000,
        _000R = 0x1000,
        _111R = 0x2000,
        RRRR = 0x3000,
        _0RRR = 0x4000,
        _1RRR = 0x5000,
        R000 = 0x6000,
        R111 = 0x7000
    };

    public enum SceGxmTextureSwizzleYUV422Mode : ushort
    {
        YUYV_CSC0 = 0x0000,
        YVYU_CSC0 = 0x1000,
        UYVY_CSC0 = 0x2000,
        VYUY_CSC0 = 0x3000,
        YUYV_CSC1 = 0x4000,
        YVYU_CSC1 = 0x5000,
        UYVY_CSC1 = 0x6000,
        VYUY_CSC1 = 0x7000
    };

    public enum SceGxmTextureSwizzleYUV420Mode : ushort
    {
        YUV_CSC0 = 0x0000,
        YVU_CSC0 = 0x1000,
        YUV_CSC1 = 0x2000,
        YVU_CSC1 = 0x3000
    };

    public enum SceGxmTextureFormat : uint
    {
        U8_000R = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode._000R,
        U8_111R = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode._111R,
        U8_RRRR = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode.RRRR,
        U8_0RRR = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode._0RRR,
        U8_1RRR = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode._1RRR,
        U8_R000 = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode.R000,
        U8_R111 = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode.R111,
        U8_R = SceGxmTextureBaseFormat.U8 | SceGxmTextureSwizzle1Mode.R,
        S8_000R = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode._000R,
        S8_111R = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode._111R,
        S8_RRRR = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode.RRRR,
        S8_0RRR = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode._0RRR,
        S8_1RRR = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode._1RRR,
        S8_R000 = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode.R000,
        S8_R111 = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode.R111,
        S8_R = SceGxmTextureBaseFormat.S8 | SceGxmTextureSwizzle1Mode.R,
        U4U4U4U4_ABGR = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode.ABGR,
        U4U4U4U4_ARGB = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode.ARGB,
        U4U4U4U4_RGBA = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode.RGBA,
        U4U4U4U4_BGRA = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode.BGRA,
        X4U4U4U4_1BGR = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode._1BGR,
        X4U4U4U4_1RGB = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode._1RGB,
        U4U4U4X4_RGB1 = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode.RGB1,
        U4U4U4X4_BGR1 = SceGxmTextureBaseFormat.U4U4U4U4 | SceGxmTextureSwizzle4Mode.BGR1,
        U8U3U3U2_ARGB = SceGxmTextureBaseFormat.U8U3U3U2,
        U1U5U5U5_ABGR = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode.ABGR,
        U1U5U5U5_ARGB = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode.ARGB,
        U5U5U5U1_RGBA = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode.RGBA,
        U5U5U5U1_BGRA = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode.BGRA,
        X1U5U5U5_1BGR = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode._1BGR,
        X1U5U5U5_1RGB = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode._1RGB,
        U5U5U5X1_RGB1 = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode.RGB1,
        U5U5U5X1_BGR1 = SceGxmTextureBaseFormat.U1U5U5U5 | SceGxmTextureSwizzle4Mode.BGR1,
        U5U6U5_BGR = SceGxmTextureBaseFormat.U5U6U5 | SceGxmTextureSwizzle3Mode.BGR,
        U5U6U5_RGB = SceGxmTextureBaseFormat.U5U6U5 | SceGxmTextureSwizzle3Mode.RGB,
        U6S5S5_BGR = SceGxmTextureBaseFormat.S5S5U6 | SceGxmTextureSwizzle3Mode.BGR,
        S5S5U6_RGB = SceGxmTextureBaseFormat.S5S5U6 | SceGxmTextureSwizzle3Mode.RGB,
        U8U8_00GR = SceGxmTextureBaseFormat.U8U8 | SceGxmTextureSwizzle2Mode._00GR,
        U8U8_GRRR = SceGxmTextureBaseFormat.U8U8 | SceGxmTextureSwizzle2Mode.GRRR,
        U8U8_RGGG = SceGxmTextureBaseFormat.U8U8 | SceGxmTextureSwizzle2Mode.RGGG,
        U8U8_GRGR = SceGxmTextureBaseFormat.U8U8 | SceGxmTextureSwizzle2Mode.GRGR,
        U8U8_00RG = SceGxmTextureBaseFormat.U8U8 | SceGxmTextureSwizzle2Mode._00RG,
        U8U8_GR = SceGxmTextureBaseFormat.U8U8 | SceGxmTextureSwizzle2Mode.GR,
        S8S8_00GR = SceGxmTextureBaseFormat.S8S8 | SceGxmTextureSwizzle2Mode._00GR,
        S8S8_GRRR = SceGxmTextureBaseFormat.S8S8 | SceGxmTextureSwizzle2Mode.GRRR,
        S8S8_RGGG = SceGxmTextureBaseFormat.S8S8 | SceGxmTextureSwizzle2Mode.RGGG,
        S8S8_GRGR = SceGxmTextureBaseFormat.S8S8 | SceGxmTextureSwizzle2Mode.GRGR,
        S8S8_00RG = SceGxmTextureBaseFormat.S8S8 | SceGxmTextureSwizzle2Mode._00RG,
        S8S8_GR = SceGxmTextureBaseFormat.S8S8 | SceGxmTextureSwizzle2Mode.GR,
        U16_000R = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode._000R,
        U16_111R = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode._111R,
        U16_RRRR = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode.RRRR,
        U16_0RRR = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode._0RRR,
        U16_1RRR = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode._1RRR,
        U16_R000 = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode.R000,
        U16_R111 = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode.R111,
        U16_R = SceGxmTextureBaseFormat.U16 | SceGxmTextureSwizzle1Mode.R,
        S16_000R = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode._000R,
        S16_111R = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode._111R,
        S16_RRRR = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode.RRRR,
        S16_0RRR = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode._0RRR,
        S16_1RRR = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode._1RRR,
        S16_R000 = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode.R000,
        S16_R111 = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode.R111,
        S16_R = SceGxmTextureBaseFormat.S16 | SceGxmTextureSwizzle1Mode.R,
        F16_000R = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode._000R,
        F16_111R = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode._111R,
        F16_RRRR = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode.RRRR,
        F16_0RRR = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode._0RRR,
        F16_1RRR = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode._1RRR,
        F16_R000 = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode.R000,
        F16_R111 = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode.R111,
        F16_R = SceGxmTextureBaseFormat.F16 | SceGxmTextureSwizzle1Mode.R,
        U8U8U8U8_ABGR = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode.ABGR,
        U8U8U8U8_ARGB = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode.ARGB,
        U8U8U8U8_RGBA = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode.RGBA,
        U8U8U8U8_BGRA = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode.BGRA,
        X8U8U8U8_1BGR = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode._1BGR,
        X8U8U8U8_1RGB = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode._1RGB,
        U8U8U8X8_RGB1 = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode.RGB1,
        U8U8U8X8_BGR1 = SceGxmTextureBaseFormat.U8U8U8U8 | SceGxmTextureSwizzle4Mode.BGR1,
        S8S8S8S8_ABGR = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode.ABGR,
        S8S8S8S8_ARGB = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode.ARGB,
        S8S8S8S8_RGBA = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode.RGBA,
        S8S8S8S8_BGRA = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode.BGRA,
        X8S8S8S8_1BGR = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode._1BGR,
        X8S8S8S8_1RGB = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode._1RGB,
        S8S8S8X8_RGB1 = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode.RGB1,
        S8S8S8X8_BGR1 = SceGxmTextureBaseFormat.S8S8S8S8 | SceGxmTextureSwizzle4Mode.BGR1,
        U2U10U10U10_ABGR = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode.ABGR,
        U2U10U10U10_ARGB = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode.ARGB,
        U10U10U10U2_RGBA = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode.RGBA,
        U10U10U10U2_BGRA = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode.BGRA,
        X2U10U10U10_1BGR = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode._1BGR,
        X2U10U10U10_1RGB = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode._1RGB,
        U10U10U10X2_RGB1 = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode.RGB1,
        U10U10U10X2_BGR1 = SceGxmTextureBaseFormat.U2U10U10U10 | SceGxmTextureSwizzle4Mode.BGR1,
        U16U16_00GR = SceGxmTextureBaseFormat.U16U16 | SceGxmTextureSwizzle2Mode._00GR,
        U16U16_GRRR = SceGxmTextureBaseFormat.U16U16 | SceGxmTextureSwizzle2Mode.GRRR,
        U16U16_RGGG = SceGxmTextureBaseFormat.U16U16 | SceGxmTextureSwizzle2Mode.RGGG,
        U16U16_GRGR = SceGxmTextureBaseFormat.U16U16 | SceGxmTextureSwizzle2Mode.GRGR,
        U16U16_00RG = SceGxmTextureBaseFormat.U16U16 | SceGxmTextureSwizzle2Mode._00RG,
        U16U16_GR = SceGxmTextureBaseFormat.U16U16 | SceGxmTextureSwizzle2Mode.GR,
        S16S16_00GR = SceGxmTextureBaseFormat.S16S16 | SceGxmTextureSwizzle2Mode._00GR,
        S16S16_GRRR = SceGxmTextureBaseFormat.S16S16 | SceGxmTextureSwizzle2Mode.GRRR,
        S16S16_RGGG = SceGxmTextureBaseFormat.S16S16 | SceGxmTextureSwizzle2Mode.RGGG,
        S16S16_GRGR = SceGxmTextureBaseFormat.S16S16 | SceGxmTextureSwizzle2Mode.GRGR,
        S16S16_00RG = SceGxmTextureBaseFormat.S16S16 | SceGxmTextureSwizzle2Mode._00RG,
        S16S16_GR = SceGxmTextureBaseFormat.S16S16 | SceGxmTextureSwizzle2Mode.GR,
        F16F16_00GR = SceGxmTextureBaseFormat.F16F16 | SceGxmTextureSwizzle2Mode._00GR,
        F16F16_GRRR = SceGxmTextureBaseFormat.F16F16 | SceGxmTextureSwizzle2Mode.GRRR,
        F16F16_RGGG = SceGxmTextureBaseFormat.F16F16 | SceGxmTextureSwizzle2Mode.RGGG,
        F16F16_GRGR = SceGxmTextureBaseFormat.F16F16 | SceGxmTextureSwizzle2Mode.GRGR,
        F16F16_00RG = SceGxmTextureBaseFormat.F16F16 | SceGxmTextureSwizzle2Mode._00RG,
        F16F16_GR = SceGxmTextureBaseFormat.F16F16 | SceGxmTextureSwizzle2Mode.GR,
        F32_000R = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode._000R,
        F32_111R = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode._111R,
        F32_RRRR = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode.RRRR,
        F32_0RRR = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode._0RRR,
        F32_1RRR = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode._1RRR,
        F32_R000 = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode.R000,
        F32_R111 = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode.R111,
        F32_R = SceGxmTextureBaseFormat.F32 | SceGxmTextureSwizzle1Mode.R,
        F32M_000R = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode._000R,
        F32M_111R = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode._111R,
        F32M_RRRR = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode.RRRR,
        F32M_0RRR = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode._0RRR,
        F32M_1RRR = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode._1RRR,
        F32M_R000 = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode.R000,
        F32M_R111 = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode.R111,
        F32M_R = SceGxmTextureBaseFormat.F32M | SceGxmTextureSwizzle1Mode.R,
        X8S8S8U8_1BGR = SceGxmTextureBaseFormat.X8S8S8U8 | SceGxmTextureSwizzle3Mode.BGR,
        X8U8S8S8_1RGB = SceGxmTextureBaseFormat.X8S8S8U8 | SceGxmTextureSwizzle3Mode.RGB,
        X8U24_SD = SceGxmTextureBaseFormat.X8U24 | SceGxmTextureSwizzle2ModeAlt.SD,
        U24X8_DS = SceGxmTextureBaseFormat.X8U24 | SceGxmTextureSwizzle2ModeAlt.DS,
        U32_000R = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode._000R,
        U32_111R = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode._111R,
        U32_RRRR = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode.RRRR,
        U32_0RRR = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode._0RRR,
        U32_1RRR = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode._1RRR,
        U32_R000 = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode.R000,
        U32_R111 = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode.R111,
        U32_R = SceGxmTextureBaseFormat.U32 | SceGxmTextureSwizzle1Mode.R,
        S32_000R = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode._000R,
        S32_111R = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode._111R,
        S32_RRRR = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode.RRRR,
        S32_0RRR = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode._0RRR,
        S32_1RRR = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode._1RRR,
        S32_R000 = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode.R000,
        S32_R111 = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode.R111,
        S32_R = SceGxmTextureBaseFormat.S32 | SceGxmTextureSwizzle1Mode.R,
        SE5M9M9M9_BGR = SceGxmTextureBaseFormat.SE5M9M9M9 | SceGxmTextureSwizzle3Mode.BGR,
        SE5M9M9M9_RGB = SceGxmTextureBaseFormat.SE5M9M9M9 | SceGxmTextureSwizzle3Mode.RGB,
        F10F11F11_BGR = SceGxmTextureBaseFormat.F11F11F10 | SceGxmTextureSwizzle3Mode.BGR,
        F11F11F10_RGB = SceGxmTextureBaseFormat.F11F11F10 | SceGxmTextureSwizzle3Mode.RGB,
        F16F16F16F16_ABGR = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode.ABGR,
        F16F16F16F16_ARGB = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode.ARGB,
        F16F16F16F16_RGBA = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode.RGBA,
        F16F16F16F16_BGRA = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode.BGRA,
        X16F16F16F16_1BGR = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode._1BGR,
        X16F16F16F16_1RGB = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode._1RGB,
        F16F16F16X16_RGB1 = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode.RGB1,
        F16F16F16X16_BGR1 = SceGxmTextureBaseFormat.F16F16F16F16 | SceGxmTextureSwizzle4Mode.BGR1,
        U16U16U16U16_ABGR = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode.ABGR,
        U16U16U16U16_ARGB = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode.ARGB,
        U16U16U16U16_RGBA = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode.RGBA,
        U16U16U16U16_BGRA = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode.BGRA,
        X16U16U16U16_1BGR = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode._1BGR,
        X16U16U16U16_1RGB = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode._1RGB,
        U16U16U16X16_RGB1 = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode.RGB1,
        U16U16U16X16_BGR1 = SceGxmTextureBaseFormat.U16U16U16U16 | SceGxmTextureSwizzle4Mode.BGR1,
        S16S16S16S16_ABGR = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode.ABGR,
        S16S16S16S16_ARGB = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode.ARGB,
        S16S16S16S16_RGBA = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode.RGBA,
        S16S16S16S16_BGRA = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode.BGRA,
        X16S16S16S16_1BGR = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode._1BGR,
        X16S16S16S16_1RGB = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode._1RGB,
        S16S16S16X16_RGB1 = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode.RGB1,
        S16S16S16X16_BGR1 = SceGxmTextureBaseFormat.S16S16S16S16 | SceGxmTextureSwizzle4Mode.BGR1,
        F32F32_00GR = SceGxmTextureBaseFormat.F32F32 | SceGxmTextureSwizzle2Mode._00GR,
        F32F32_GRRR = SceGxmTextureBaseFormat.F32F32 | SceGxmTextureSwizzle2Mode.GRRR,
        F32F32_RGGG = SceGxmTextureBaseFormat.F32F32 | SceGxmTextureSwizzle2Mode.RGGG,
        F32F32_GRGR = SceGxmTextureBaseFormat.F32F32 | SceGxmTextureSwizzle2Mode.GRGR,
        F32F32_00RG = SceGxmTextureBaseFormat.F32F32 | SceGxmTextureSwizzle2Mode._00RG,
        F32F32_GR = SceGxmTextureBaseFormat.F32F32 | SceGxmTextureSwizzle2Mode.GR,
        U32U32_00GR = SceGxmTextureBaseFormat.U32U32 | SceGxmTextureSwizzle2Mode._00GR,
        U32U32_GRRR = SceGxmTextureBaseFormat.U32U32 | SceGxmTextureSwizzle2Mode.GRRR,
        U32U32_RGGG = SceGxmTextureBaseFormat.U32U32 | SceGxmTextureSwizzle2Mode.RGGG,
        U32U32_GRGR = SceGxmTextureBaseFormat.U32U32 | SceGxmTextureSwizzle2Mode.GRGR,
        U32U32_00RG = SceGxmTextureBaseFormat.U32U32 | SceGxmTextureSwizzle2Mode._00RG,
        U32U32_GR = SceGxmTextureBaseFormat.U32U32 | SceGxmTextureSwizzle2Mode.GR,
        PVRT2BPP_ABGR = SceGxmTextureBaseFormat.PVRT2BPP | SceGxmTextureSwizzle4Mode.ABGR,
        PVRT2BPP_1BGR = SceGxmTextureBaseFormat.PVRT2BPP | SceGxmTextureSwizzle4Mode._1BGR,
        PVRT4BPP_ABGR = SceGxmTextureBaseFormat.PVRT4BPP | SceGxmTextureSwizzle4Mode.ABGR,
        PVRT4BPP_1BGR = SceGxmTextureBaseFormat.PVRT4BPP | SceGxmTextureSwizzle4Mode._1BGR,
        PVRTII2BPP_ABGR = SceGxmTextureBaseFormat.PVRTII2BPP | SceGxmTextureSwizzle4Mode.ABGR,
        PVRTII2BPP_1BGR = SceGxmTextureBaseFormat.PVRTII2BPP | SceGxmTextureSwizzle4Mode._1BGR,
        PVRTII4BPP_ABGR = SceGxmTextureBaseFormat.PVRTII4BPP | SceGxmTextureSwizzle4Mode.ABGR,
        PVRTII4BPP_1BGR = SceGxmTextureBaseFormat.PVRTII4BPP | SceGxmTextureSwizzle4Mode._1BGR,
        UBC1_ABGR = SceGxmTextureBaseFormat.UBC1 | SceGxmTextureSwizzle4Mode.ABGR,
        UBC2_ABGR = SceGxmTextureBaseFormat.UBC2 | SceGxmTextureSwizzle4Mode.ABGR,
        UBC3_ABGR = SceGxmTextureBaseFormat.UBC3 | SceGxmTextureSwizzle4Mode.ABGR,
        YUV420P2_CSC0 = SceGxmTextureBaseFormat.YUV420P2 | SceGxmTextureSwizzleYUV420Mode.YUV_CSC0,
        YVU420P2_CSC0 = SceGxmTextureBaseFormat.YUV420P2 | SceGxmTextureSwizzleYUV420Mode.YVU_CSC0,
        YUV420P2_CSC1 = SceGxmTextureBaseFormat.YUV420P2 | SceGxmTextureSwizzleYUV420Mode.YUV_CSC1,
        YVU420P2_CSC1 = SceGxmTextureBaseFormat.YUV420P2 | SceGxmTextureSwizzleYUV420Mode.YVU_CSC1,
        YUV420P3_CSC0 = SceGxmTextureBaseFormat.YUV420P3 | SceGxmTextureSwizzleYUV420Mode.YUV_CSC0,
        YVU420P3_CSC0 = SceGxmTextureBaseFormat.YUV420P3 | SceGxmTextureSwizzleYUV420Mode.YVU_CSC0,
        YUV420P3_CSC1 = SceGxmTextureBaseFormat.YUV420P3 | SceGxmTextureSwizzleYUV420Mode.YUV_CSC1,
        YVU420P3_CSC1 = SceGxmTextureBaseFormat.YUV420P3 | SceGxmTextureSwizzleYUV420Mode.YVU_CSC1,
        YUYV422_CSC0 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.YUYV_CSC0,
        YVYU422_CSC0 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.YVYU_CSC0,
        UYVY422_CSC0 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.UYVY_CSC0,
        VYUY422_CSC0 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.VYUY_CSC0,
        YUYV422_CSC1 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.YUYV_CSC1,
        YVYU422_CSC1 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.YVYU_CSC1,
        UYVY422_CSC1 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.UYVY_CSC1,
        VYUY422_CSC1 = SceGxmTextureBaseFormat.YUV422 | SceGxmTextureSwizzleYUV422Mode.VYUY_CSC1,
        P4_ABGR = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode.ABGR,
        P4_ARGB = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode.ARGB,
        P4_RGBA = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode.RGBA,
        P4_BGRA = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode.BGRA,
        P4_1BGR = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode._1BGR,
        P4_1RGB = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode._1RGB,
        P4_RGB1 = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode.RGB1,
        P4_BGR1 = SceGxmTextureBaseFormat.P4 | SceGxmTextureSwizzle4Mode.BGR1,
        P8_ABGR = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode.ABGR,
        P8_ARGB = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode.ARGB,
        P8_RGBA = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode.RGBA,
        P8_BGRA = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode.BGRA,
        P8_1BGR = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode._1BGR,
        P8_1RGB = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode._1RGB,
        P8_RGB1 = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode.RGB1,
        P8_BGR1 = SceGxmTextureBaseFormat.P8 | SceGxmTextureSwizzle4Mode.BGR1,
        U8U8U8_BGR = SceGxmTextureBaseFormat.U8U8U8 | SceGxmTextureSwizzle3Mode.BGR,
        U8U8U8_RGB = SceGxmTextureBaseFormat.U8U8U8 | SceGxmTextureSwizzle3Mode.RGB,
        S8S8S8_BGR = SceGxmTextureBaseFormat.S8S8S8 | SceGxmTextureSwizzle3Mode.BGR,
        S8S8S8_RGB = SceGxmTextureBaseFormat.S8S8S8 | SceGxmTextureSwizzle3Mode.RGB,
        U2F10F10F10_ABGR = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode.ABGR,
        U2F10F10F10_ARGB = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode.ARGB,
        F10F10F10U2_RGBA = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode.RGBA,
        F10F10F10U2_BGRA = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode.BGRA,
        X2F10F10F10_1BGR = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode._1BGR,
        X2F10F10F10_1RGB = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode._1RGB,
        F10F10F10X2_RGB1 = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode.RGB1,
        F10F10F10X2_BGR1 = SceGxmTextureBaseFormat.U2F10F10F10 | SceGxmTextureSwizzle4Mode.BGR1,
        L8 = SceGxmTextureFormat.U8_1RRR,
        A8 = SceGxmTextureFormat.U8_R000,
        R8 = SceGxmTextureFormat.U8_000R,
        A4R4G4B4 = SceGxmTextureFormat.U4U4U4U4_ARGB,
        A1R5G5B5 = SceGxmTextureFormat.U1U5U5U5_ARGB,
        R5G6B5 = SceGxmTextureFormat.U5U6U5_RGB,
        A8L8 = SceGxmTextureFormat.U8U8_GRRR,
        L8A8 = SceGxmTextureFormat.U8U8_RGGG,
        G8R8 = SceGxmTextureFormat.U8U8_00GR,
        L16 = SceGxmTextureFormat.U16_1RRR,
        A16 = SceGxmTextureFormat.U16_R000,
        R16 = SceGxmTextureFormat.U16_000R,
        D16 = SceGxmTextureFormat.U16_R,
        LF16 = SceGxmTextureFormat.F16_1RRR,
        AF16 = SceGxmTextureFormat.F16_R000,
        RF16 = SceGxmTextureFormat.F16_000R,
        A8R8G8B8 = SceGxmTextureFormat.U8U8U8U8_ARGB,
        A8B8G8R8 = SceGxmTextureFormat.U8U8U8U8_ABGR,
        AF16LF16 = SceGxmTextureFormat.F16F16_GRRR,
        LF16AF16 = SceGxmTextureFormat.F16F16_RGGG,
        GF16RF16 = SceGxmTextureFormat.F16F16_00GR,
        LF32M = SceGxmTextureFormat.F32M_1RRR,
        AF32M = SceGxmTextureFormat.F32M_R000,
        RF32M = SceGxmTextureFormat.F32M_000R,
        DF32M = SceGxmTextureFormat.F32M_R,
        VYUY = SceGxmTextureFormat.VYUY422_CSC0,
        YVYU = SceGxmTextureFormat.YVYU422_CSC0,
        UBC1 = SceGxmTextureFormat.UBC1_ABGR,
        UBC2 = SceGxmTextureFormat.UBC2_ABGR,
        UBC3 = SceGxmTextureFormat.UBC3_ABGR,
        PVRT2BPP = SceGxmTextureFormat.PVRT2BPP_ABGR,
        PVRT4BPP = SceGxmTextureFormat.PVRT4BPP_ABGR,
        PVRTII2BPP = SceGxmTextureFormat.PVRTII2BPP_ABGR,
        PVRTII4BPP = SceGxmTextureFormat.PVRTII4BPP_ABGR
    };

    public enum SceGxmTextureType : uint
    {
        Swizzled = 0x00000000,
        Cube = 0x40000000,
        Linear = 0x60000000,
        Tiled = 0x80000000,
        LinearStrided = 0xC0000000
    };

    public class TypeNotImplementedException : Exception
    {
        public SceGxmTextureType Type { get; private set; }

        public TypeNotImplementedException(SceGxmTextureType type) : base() { this.Type = type; }
    }

    public class FormatNotImplementedException : Exception
    {
        public SceGxmTextureFormat Format { get; private set; }

        public FormatNotImplementedException(SceGxmTextureFormat format) : base() { this.Format = format; }
    }

    public class VersionNotImplementedException : Exception
    {
        public uint Version { get; private set; }

        public VersionNotImplementedException(uint version) : base() { this.Version = version; }
    }

    public class UnknownMagicException : Exception
    {
        public UnknownMagicException() : base() { }
        public UnknownMagicException(string message) : base(message) { }
        public UnknownMagicException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class PaletteNotImplementedException : Exception
    {
        public SceGxmTextureFormat Format { get; private set; }

        public PaletteNotImplementedException(SceGxmTextureFormat format) : base() { this.Format = format; }
    }

    public class CommandLineArgsException : Exception
    {
        public string ExpectedArgs { get; private set; }

        public CommandLineArgsException(string expectedArgs) : base() { this.ExpectedArgs = expectedArgs; }
    }

    public class SceGxtHeader
    {
        public const string ExpectedMagicNumber = "GXT\0";

        public string MagicNumber { get; private set; } = string.Empty;
        public uint Version { get; private set; }
        public uint NumTextures { get; private set; }
        public uint TextureDataOffset { get; private set; }
        public uint TextureDataSize { get; private set; }
        public uint NumP4Palettes { get; private set; }
        public uint NumP8Palettes { get; private set; }
        public uint Padding { get; private set; }

        public SceGxtHeader(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            MagicNumber = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (MagicNumber != ExpectedMagicNumber)
                throw new UnknownMagicException($"出乎意料的'GTX{MagicNumber}");

            Version = reader.ReadUInt32();
            NumTextures = reader.ReadUInt32();
            TextureDataOffset = reader.ReadUInt32();
            TextureDataSize = reader.ReadUInt32();
            NumP4Palettes = reader.ReadUInt32();
            NumP8Palettes = reader.ReadUInt32();
            Padding = reader.ReadUInt32();
        }
    }

    public abstract class SceGxtTextureInfo
    {
        public uint DataOffset { get; private set; }
        public uint DataSize { get; private set; }
        public int PaletteIndex { get; private set; }
        public uint Flags { get; private set; }
        public uint[] ControlWords { get; private set; } = Array.Empty<uint>();

        public abstract SceGxmTextureType GetTextureType();
        public abstract SceGxmTextureFormat GetTextureFormat();
        public abstract ushort GetWidth();
        public abstract ushort GetHeight();

        public SceGxmTextureBaseFormat GetTextureBaseFormat()
        {
            return (SceGxmTextureBaseFormat)((uint)GetTextureFormat() & 0xFFFF0000);
        }

        public SceGxtTextureInfo(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            DataOffset = reader.ReadUInt32();
            DataSize = reader.ReadUInt32();
            PaletteIndex = reader.ReadInt32();
            Flags = reader.ReadUInt32();
            ControlWords = new uint[4];
            for (int i = 0; i < ControlWords.Length; i++) ControlWords[i] = reader.ReadUInt32();
        }

        public ushort GetWidthRounded()
        {
            int roundedWidth = 1;
            while (roundedWidth < GetWidth()) roundedWidth *= 2;
            return (ushort)roundedWidth;
        }

        public ushort GetHeightRounded()
        {
            int roundedHeight = 1;
            while (roundedHeight < GetHeight()) roundedHeight *= 2;
            return (ushort)roundedHeight;
        }
    }

    public class SceGxtTextureInfoV301 : SceGxtTextureInfo
    {
        public SceGxtTextureInfoV301(Stream stream) : base(stream) { }

        public override SceGxmTextureType GetTextureType() { return (SceGxmTextureType)ControlWords[0]; }
        public override SceGxmTextureFormat GetTextureFormat() { return (SceGxmTextureFormat)ControlWords[1]; }
        public override ushort GetWidth() { return (ushort)(ControlWords[2] & 0xFFFF); }
        public override ushort GetHeight() { return (ushort)(ControlWords[2] >> 16); }
    }

    public class SceGxtTextureInfoV201 : SceGxtTextureInfo
    {
        public SceGxtTextureInfoV201(Stream stream) : base(stream) { }

        public override SceGxmTextureType GetTextureType() { return (SceGxmTextureType)ControlWords[2]; }
        public override SceGxmTextureFormat GetTextureFormat() { return (SceGxmTextureFormat)(0x80000000 | ((ControlWords[1] >> 24) & 0xF) << 24); }
        public override ushort GetWidth() { return (ushort)(1 << (ushort)((ControlWords[1] >> 16) & 0xF)); }
        public override ushort GetHeight() { return (ushort)(1 << (ushort)((ControlWords[1] >> 0) & 0xF)); }
    }

    public class SceGxtTextureInfoV101 : SceGxtTextureInfo
    {
        public SceGxtTextureInfoV101(Stream stream) : base(stream) { }

        public override SceGxmTextureType GetTextureType() { return (SceGxmTextureType)ControlWords[2]; }
        public override SceGxmTextureFormat GetTextureFormat() { return (SceGxmTextureFormat)(0x80000000 | ((ControlWords[1] >> 24) & 0xF) << 24); }
        public override ushort GetWidth() { return (ushort)(1 << (ushort)((ControlWords[1] >> 16) & 0xF)); }
        public override ushort GetHeight() { return (ushort)(1 << (ushort)((ControlWords[1] >> 0) & 0xF)); }
    }

    public class BUVEntry
    {
        public ushort X { get; private set; }
        public ushort Y { get; private set; }
        public ushort Width { get; private set; }
        public ushort Height { get; private set; }
        public short PaletteIndex { get; private set; }
        public ushort Unknown0x0A { get; private set; }

        public BUVEntry(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            X = reader.ReadUInt16();
            Y = reader.ReadUInt16();
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            PaletteIndex = reader.ReadInt16();
            Unknown0x0A = reader.ReadUInt16();
        }
    }

    public class BUVChunk
    {
        public const string ExpectedMagicNumber = "BUV\0";

        public string MagicNumber { get; private set; } = string.Empty;
        public uint NumEntries { get; private set; }
        public BUVEntry[] Entries { get; private set; } = Array.Empty<BUVEntry>();

        public BUVChunk(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            MagicNumber = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (MagicNumber != ExpectedMagicNumber)
                throw new UnknownMagicException($"出乎意料的'BUV{MagicNumber}");

            NumEntries = reader.ReadUInt32();
            Entries = new BUVEntry[NumEntries];
            for (int i = 0; i < Entries.Length; i++) Entries[i] = new BUVEntry(stream);
        }
    }

    public static class DXTx
    {
        static readonly int[] dxtOrder = new int[]
        {
            0, 2, 8, 10,
            1, 3, 9, 11,
            4, 6, 12, 14,
            5, 7, 13, 15
        };

        public static byte[] Decompress(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 8];

            int pixelOffset = 0;
            for (int y = 0; y < info.GetHeightRounded(); y += 4)
            {
                for (int x = 0; x < info.GetWidthRounded(); x += 4)
                {
                    byte[] decodedBlock = DecompressDxtBlock(reader, info.GetTextureBaseFormat());
                    for (int b = 0; b < dxtOrder.Length; b++) Buffer.BlockCopy(decodedBlock, b * 4, pixelData, pixelOffset + (dxtOrder[b] * 4), 4);
                    pixelOffset += decodedBlock.Length;
                }
            }

            return pixelData;
        }

        private static byte[] DecompressDxtBlock(BinaryReader reader, SceGxmTextureBaseFormat format)
        {
            byte[] outputData = new byte[(4 * 4) * 4];
            byte[]? alphaData = null;

            if (format != SceGxmTextureBaseFormat.UBC1)
                alphaData = DecompressDxtAlpha(reader, format);

            byte[] colorData = DecompressDxtColor(reader, format);

            for (int i = 0; i < colorData.Length; i += 4)
            {
                outputData[i] = colorData[i];
                outputData[i + 1] = colorData[i + 1];
                outputData[i + 2] = colorData[i + 2];
                outputData[i + 3] = (alphaData != null ? alphaData[i + 3] : colorData[i + 3]);
            }

            return outputData;
        }

        private static byte[] DecompressDxtColor(BinaryReader reader, SceGxmTextureBaseFormat format)
        {
            byte[] colorOut = new byte[(4 * 4) * 4];

            ushort color0 = reader.ReadUInt16();
            ushort color1 = reader.ReadUInt16();
            uint bits = reader.ReadUInt32();

            byte c0r, c0g, c0b, c1r, c1g, c1b;
            UnpackRgb565(color0, out c0r, out c0g, out c0b);
            UnpackRgb565(color1, out c1r, out c1g, out c1b);

            byte[] bitsExt = new byte[16];
            for (int i = 0; i < bitsExt.Length; i++)
                bitsExt[i] = (byte)((bits >> (i * 2)) & 0x3);

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    byte code = bitsExt[(y * 4) + x];
                    int destOffset = ((y * 4) + x) * 4;

                    colorOut[destOffset + 3] = 0xFF;

                    if (format == SceGxmTextureBaseFormat.UBC1 && color0 <= color1)
                    {
                        switch (code)
                        {
                            case 0x00:
                                colorOut[destOffset + 0] = c0b;
                                colorOut[destOffset + 1] = c0g;
                                colorOut[destOffset + 2] = c0r;
                                break;

                            case 0x01:
                                colorOut[destOffset + 0] = c1b;
                                colorOut[destOffset + 1] = c1g;
                                colorOut[destOffset + 2] = c1r;
                                break;

                            case 0x02:
                                colorOut[destOffset + 0] = (byte)((c0b + c1b) / 2);
                                colorOut[destOffset + 1] = (byte)((c0g + c1g) / 2);
                                colorOut[destOffset + 2] = (byte)((c0r + c1r) / 2);
                                break;

                            case 0x03:
                                colorOut[destOffset + 0] = 0;
                                colorOut[destOffset + 1] = 0;
                                colorOut[destOffset + 2] = 0;
                                break;
                        }
                    }
                    else
                    {
                        switch (code)
                        {
                            case 0x00:
                                colorOut[destOffset + 0] = c0b;
                                colorOut[destOffset + 1] = c0g;
                                colorOut[destOffset + 2] = c0r;
                                break;

                            case 0x01:
                                colorOut[destOffset + 0] = c1b;
                                colorOut[destOffset + 1] = c1g;
                                colorOut[destOffset + 2] = c1r;
                                break;

                            case 0x02:
                                colorOut[destOffset + 0] = (byte)((2 * c0b + c1b) / 3);
                                colorOut[destOffset + 1] = (byte)((2 * c0g + c1g) / 3);
                                colorOut[destOffset + 2] = (byte)((2 * c0r + c1r) / 3);
                                break;

                            case 0x03:
                                colorOut[destOffset + 0] = (byte)((c0b + 2 * c1b) / 3);
                                colorOut[destOffset + 1] = (byte)((c0g + 2 * c1g) / 3);
                                colorOut[destOffset + 2] = (byte)((c0r + 2 * c1r) / 3);
                                break;
                        }
                    }
                }
            }

            return colorOut;
        }

        private static void UnpackRgb565(ushort rgb565, out byte r, out byte g, out byte b)
        {
            r = (byte)((rgb565 & 0xF800) >> 11);
            r = (byte)((r << 3) | (r >> 2));
            g = (byte)((rgb565 & 0x07E0) >> 5);
            g = (byte)((g << 2) | (g >> 4));
            b = (byte)(rgb565 & 0x1F);
            b = (byte)((b << 3) | (b >> 2));
        }

        private static byte[] DecompressDxtAlpha(BinaryReader reader, SceGxmTextureBaseFormat format)
        {
            byte[] alphaOut = new byte[(4 * 4) * 4];

            if (format == SceGxmTextureBaseFormat.UBC2)
            {
                ulong alpha = reader.ReadUInt64();
                for (int i = 0; i < alphaOut.Length; i += 4)
                {
                    alphaOut[i + 3] = (byte)(((alpha & 0xF) << 4) | (alpha & 0xF));
                    alpha >>= 4;
                }
            }
            else if (format == SceGxmTextureBaseFormat.UBC3)
            {
                byte alpha0 = reader.ReadByte();
                byte alpha1 = reader.ReadByte();
                byte bits_5 = reader.ReadByte();
                byte bits_4 = reader.ReadByte();
                byte bits_3 = reader.ReadByte();
                byte bits_2 = reader.ReadByte();
                byte bits_1 = reader.ReadByte();
                byte bits_0 = reader.ReadByte();

                ulong bits = (ulong)(((ulong)bits_0 << 40) | ((ulong)bits_1 << 32) | ((ulong)bits_2 << 24) | ((ulong)bits_3 << 16) | ((ulong)bits_4 << 8) | (ulong)bits_5);

                byte[] bitsExt = new byte[16];
                for (int i = 0; i < bitsExt.Length; i++)
                    bitsExt[i] = (byte)((bits >> (i * 3)) & 0x7);

                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        byte code = bitsExt[(y * 4) + x];
                        int destOffset = (((y * 4) + x) * 4) + 3;

                        if (alpha0 > alpha1)
                        {
                            switch (code)
                            {
                                case 0x00: alphaOut[destOffset] = alpha0; break;
                                case 0x01: alphaOut[destOffset] = alpha1; break;
                                case 0x02: alphaOut[destOffset] = (byte)((6 * alpha0 + 1 * alpha1) / 7); break;
                                case 0x03: alphaOut[destOffset] = (byte)((5 * alpha0 + 2 * alpha1) / 7); break;
                                case 0x04: alphaOut[destOffset] = (byte)((4 * alpha0 + 3 * alpha1) / 7); break;
                                case 0x05: alphaOut[destOffset] = (byte)((3 * alpha0 + 4 * alpha1) / 7); break;
                                case 0x06: alphaOut[destOffset] = (byte)((2 * alpha0 + 5 * alpha1) / 7); break;
                                case 0x07: alphaOut[destOffset] = (byte)((1 * alpha0 + 6 * alpha1) / 7); break;
                            }
                        }
                        else
                        {
                            switch (code)
                            {
                                case 0x00: alphaOut[destOffset] = alpha0; break;
                                case 0x01: alphaOut[destOffset] = alpha1; break;
                                case 0x02: alphaOut[destOffset] = (byte)((4 * alpha0 + 1 * alpha1) / 5); break;
                                case 0x03: alphaOut[destOffset] = (byte)((3 * alpha0 + 2 * alpha1) / 5); break;
                                case 0x04: alphaOut[destOffset] = (byte)((2 * alpha0 + 3 * alpha1) / 5); break;
                                case 0x05: alphaOut[destOffset] = (byte)((1 * alpha0 + 4 * alpha1) / 5); break;
                                case 0x06: alphaOut[destOffset] = 0x00; break;
                                case 0x07: alphaOut[destOffset] = 0xFF; break;
                            }
                        }
                    }
                }
            }

            return alphaOut;
        }
    }

    public static class PVRTC
    {
        public static byte[] Decompress(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 8];

            PVRTDecompressPVRTC(
                reader.ReadBytes((int)info.DataSize),
                (info.GetTextureBaseFormat() == SceGxmTextureBaseFormat.PVRT2BPP || info.GetTextureBaseFormat() == SceGxmTextureBaseFormat.PVRTII2BPP ? 1 : 0),
                info.GetWidth(),
                info.GetHeight(),
                (info.GetTextureBaseFormat() == SceGxmTextureBaseFormat.PVRTII2BPP || info.GetTextureBaseFormat() == SceGxmTextureBaseFormat.PVRTII4BPP ? 1 : 0),
                ref pixelData);

            return pixelData;
        }

        struct Pixel32
        {
            public byte red, green, blue, alpha;
        };

        struct Pixel128S
        {
            public int red, green, blue, alpha;
        };

        struct PVRTCWord
        {
            public uint u32ModulationData;
            public uint u32ColorData;
        };

        struct PVRTCWordIndices
        {
            public int[] P, Q, R, S;

            public PVRTCWordIndices(int p0, int p1, int q0, int q1, int r0, int r1, int s0, int s1)
            {
                P = new int[2];
                P[0] = p0;
                P[1] = p1;
                Q = new int[2];
                Q[0] = q0;
                Q[1] = q1;
                R = new int[2];
                R[0] = r0;
                R[1] = r1;
                S = new int[2];
                S[0] = s0;
                S[1] = s1;
            }
        };

        static Pixel32 getColorA(uint u32ColorData)
        {
            Pixel32 color;

            if ((u32ColorData & 0x8000) != 0)
            {
                color.red = (byte)((u32ColorData & 0x7c00) >> 10);
                color.green = (byte)((u32ColorData & 0x3e0) >> 5);
                color.blue = (byte)((u32ColorData & 0x1e) | ((u32ColorData & 0x1e) >> 4));
                color.alpha = (byte)0xf;
            }
            else
            {
                color.red = (byte)(((u32ColorData & 0xf00) >> 7) | ((u32ColorData & 0xf00) >> 11));
                color.green = (byte)(((u32ColorData & 0xf0) >> 3) | ((u32ColorData & 0xf0) >> 7));
                color.blue = (byte)(((u32ColorData & 0xe) << 1) | ((u32ColorData & 0xe) >> 2));
                color.alpha = (byte)((u32ColorData & 0x7000) >> 11);
            }

            return color;
        }

        static Pixel32 getColorB(uint u32ColorData)
        {
            Pixel32 color;

            if ((u32ColorData & 0x80000000) != 0)
            {
                color.red = (byte)((u32ColorData & 0x7c000000) >> 26);
                color.green = (byte)((u32ColorData & 0x3e00000) >> 21);
                color.blue = (byte)((u32ColorData & 0x1f0000) >> 16);
                color.alpha = (byte)0xf;
            }
            else
            {
                color.red = (byte)(((u32ColorData & 0xf000000) >> 23) | ((u32ColorData & 0xf000000) >> 27));
                color.green = (byte)(((u32ColorData & 0xf00000) >> 19) | ((u32ColorData & 0xf00000) >> 23));
                color.blue = (byte)(((u32ColorData & 0xf0000) >> 15) | ((u32ColorData & 0xf0000) >> 19));
                color.alpha = (byte)((u32ColorData & 0x70000000) >> 27);
            }

            return color;
        }

        static void interpolateColors(Pixel32 P, Pixel32 Q, Pixel32 R, Pixel32 S, ref Pixel128S[] pPixel, byte ui8Bpp)
        {
            uint ui32WordWidth = 4;
            uint ui32WordHeight = 4;
            if (ui8Bpp == 2)
            {
                ui32WordWidth = 8;
            }

            Pixel128S hP = new Pixel128S() { red = (int)P.red, green = (int)P.green, blue = (int)P.blue, alpha = (int)P.alpha };
            Pixel128S hQ = new Pixel128S() { red = (int)Q.red, green = (int)Q.green, blue = (int)Q.blue, alpha = (int)Q.alpha };
            Pixel128S hR = new Pixel128S() { red = (int)R.red, green = (int)R.green, blue = (int)R.blue, alpha = (int)R.alpha };
            Pixel128S hS = new Pixel128S() { red = (int)S.red, green = (int)S.green, blue = (int)S.blue, alpha = (int)S.alpha };

            Pixel128S QminusP = new Pixel128S() { red = hQ.red - hP.red, green = hQ.green - hP.green, blue = hQ.blue - hP.blue, alpha = hQ.alpha - hP.alpha };
            Pixel128S SminusR = new Pixel128S() { red = hS.red - hR.red, green = hS.green - hR.green, blue = hS.blue - hR.blue, alpha = hS.alpha - hR.alpha };

            hP.red *= (int)ui32WordWidth;
            hP.green *= (int)ui32WordWidth;
            hP.blue *= (int)ui32WordWidth;
            hP.alpha *= (int)ui32WordWidth;
            hR.red *= (int)ui32WordWidth;
            hR.green *= (int)ui32WordWidth;
            hR.blue *= (int)ui32WordWidth;
            hR.alpha *= (int)ui32WordWidth;

            if (ui8Bpp == 2)
            {
                for (uint x = 0; x < ui32WordWidth; x++)
                {
                    Pixel128S result = new Pixel128S() { red = 4 * hP.red, green = 4 * hP.green, blue = 4 * hP.blue, alpha = 4 * hP.alpha };
                    Pixel128S dY = new Pixel128S() { red = hR.red - hP.red, green = hR.green - hP.green, blue = hR.blue - hP.blue, alpha = hR.alpha - hP.alpha };

                    for (uint y = 0; y < ui32WordHeight; y++)
                    {
                        pPixel[y * ui32WordWidth + x].red = (int)((result.red >> 7) + (result.red >> 2));
                        pPixel[y * ui32WordWidth + x].green = (int)((result.green >> 7) + (result.green >> 2));
                        pPixel[y * ui32WordWidth + x].blue = (int)((result.blue >> 7) + (result.blue >> 2));
                        pPixel[y * ui32WordWidth + x].alpha = (int)((result.alpha >> 5) + (result.alpha >> 1));

                        result.red += dY.red;
                        result.green += dY.green;
                        result.blue += dY.blue;
                        result.alpha += dY.alpha;
                    }

                    hP.red += QminusP.red;
                    hP.green += QminusP.green;
                    hP.blue += QminusP.blue;
                    hP.alpha += QminusP.alpha;

                    hR.red += SminusR.red;
                    hR.green += SminusR.green;
                    hR.blue += SminusR.blue;
                    hR.alpha += SminusR.alpha;
                }
            }
            else
            {
                for (uint y = 0; y < ui32WordHeight; y++)
                {
                    Pixel128S result = new Pixel128S() { red = 4 * hP.red, green = 4 * hP.green, blue = 4 * hP.blue, alpha = 4 * hP.alpha };
                    Pixel128S dY = new Pixel128S() { red = hR.red - hP.red, green = hR.green - hP.green, blue = hR.blue - hP.blue, alpha = hR.alpha - hP.alpha };

                    for (uint x = 0; x < ui32WordWidth; x++)
                    {
                        pPixel[y * ui32WordWidth + x].red = (int)((result.red >> 6) + (result.red >> 1));
                        pPixel[y * ui32WordWidth + x].green = (int)((result.green >> 6) + (result.green >> 1));
                        pPixel[y * ui32WordWidth + x].blue = (int)((result.blue >> 6) + (result.blue >> 1));
                        pPixel[y * ui32WordWidth + x].alpha = (int)((result.alpha >> 4) + (result.alpha));

                        result.red += dY.red;
                        result.green += dY.green;
                        result.blue += dY.blue;
                        result.alpha += dY.alpha;
                    }

                    hP.red += QminusP.red;
                    hP.green += QminusP.green;
                    hP.blue += QminusP.blue;
                    hP.alpha += QminusP.alpha;

                    hR.red += SminusR.red;
                    hR.green += SminusR.green;
                    hR.blue += SminusR.blue;
                    hR.alpha += SminusR.alpha;
                }
            }
        }

        static void unpackModulations(PVRTCWord word, int offsetX, int offsetY, int[][] i32ModulationValues, int[][] i32ModulationModes, byte ui8Bpp)
        {
            uint WordModMode = word.u32ColorData & 0x1;
            uint ModulationBits = word.u32ModulationData;

            if (ui8Bpp == 2)
            {
                if (WordModMode != 0)
                {
                    if ((ModulationBits & 0x1) != 0)
                    {
                        if ((ModulationBits & (0x1 << 20)) != 0)
                        {
                            WordModMode = 3;
                        }
                        else
                        {
                            WordModMode = 2;
                        }

                        if ((ModulationBits & (0x1 << 21)) != 0)
                        {
                            ModulationBits |= (0x1 << 20);
                        }
                        else
                        {
                            ModulationBits &= ~((uint)0x1 << 20);
                        }
                    }

                    if ((ModulationBits & 0x2) != 0)
                    {
                        ModulationBits |= 0x1;
                    }
                    else
                    {
                        ModulationBits &= ~(uint)0x1;
                    }

                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            i32ModulationModes[x + offsetX][y + offsetY] = (int)WordModMode;

                            if (((x ^ y) & 1) == 0)
                            {
                                i32ModulationValues[x + offsetX][y + offsetY] = (int)(ModulationBits & 3);
                                ModulationBits >>= 2;
                            }
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            i32ModulationModes[x + offsetX][y + offsetY] = (int)WordModMode;

                            if ((ModulationBits & 1) != 0)
                            {
                                i32ModulationValues[x + offsetX][y + offsetY] = 0x3;
                            }
                            else
                            {
                                i32ModulationValues[x + offsetX][y + offsetY] = 0x0;
                            }
                            ModulationBits >>= 1;
                        }
                    }
                }
            }
            else
            {
                if (WordModMode != 0)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            i32ModulationValues[y + offsetY][x + offsetX] = (int)(ModulationBits & 3);
                            if (i32ModulationValues[y + offsetY][x + offsetX] == 1)
                            {
                                i32ModulationValues[y + offsetY][x + offsetX] = 4;
                            }
                            else if (i32ModulationValues[y + offsetY][x + offsetX] == 2)
                            {
                                i32ModulationValues[y + offsetY][x + offsetX] = 14;
                            }
                            else if (i32ModulationValues[y + offsetY][x + offsetX] == 3)
                            {
                                i32ModulationValues[y + offsetY][x + offsetX] = 8;
                            }
                            ModulationBits >>= 2;
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            i32ModulationValues[y + offsetY][x + offsetX] = (int)(ModulationBits & 3);
                            i32ModulationValues[y + offsetY][x + offsetX] *= 3;
                            if (i32ModulationValues[y + offsetY][x + offsetX] > 3) { i32ModulationValues[y + offsetY][x + offsetX] -= 1; }
                            ModulationBits >>= 2;
                        }
                    }
                }
            }
        }

        static int getModulationValues(int[][] i32ModulationValues, int[][] i32ModulationModes, uint xPos, uint yPos, byte ui8Bpp)
        {
            if (ui8Bpp == 2)
            {
                int[] RepVals0 = { 0, 3, 5, 8 };

                if (i32ModulationModes[xPos][yPos] == 0)
                {
                    return RepVals0[i32ModulationValues[xPos][yPos]];
                }
                else
                {
                    if (((xPos ^ yPos) & 1) == 0)
                    {
                        return RepVals0[i32ModulationValues[xPos][yPos]];
                    }
                    else if (i32ModulationModes[xPos][yPos] == 1)
                    {
                        return (RepVals0[i32ModulationValues[xPos][yPos - 1]] +
                                RepVals0[i32ModulationValues[xPos][yPos + 1]] +
                                RepVals0[i32ModulationValues[xPos - 1][yPos]] +
                                RepVals0[i32ModulationValues[xPos + 1][yPos]] + 2) / 4;
                    }
                    else if (i32ModulationModes[xPos][yPos] == 2)
                    {
                        return (RepVals0[i32ModulationValues[xPos - 1][yPos]] +
                                RepVals0[i32ModulationValues[xPos + 1][yPos]] + 1) / 2;
                    }
                    else
                    {
                        return (RepVals0[i32ModulationValues[xPos][yPos - 1]] +
                                RepVals0[i32ModulationValues[xPos][yPos + 1]] + 1) / 2;
                    }
                }
            }
            else if (ui8Bpp == 4)
            {
                return i32ModulationValues[xPos][yPos];
            }

            return 0;
        }

        static void pvrtcGetDecompressedPixels(PVRTCWord P, PVRTCWord Q, PVRTCWord R, PVRTCWord S, ref Pixel32[] pColorData, byte ui8Bpp)
        {
            int[][] i32ModulationValues = new int[16][];
            for (int i = 0; i < i32ModulationValues.Length; i++) i32ModulationValues[i] = new int[8];
            int[][] i32ModulationModes = new int[16][];
            for (int i = 0; i < i32ModulationModes.Length; i++) i32ModulationModes[i] = new int[8];

            Pixel128S[] upscaledColorA = new Pixel128S[32];
            Pixel128S[] upscaledColorB = new Pixel128S[32];

            uint ui32WordWidth = 4;
            uint ui32WordHeight = 4;
            if (ui8Bpp == 2)
            {
                ui32WordWidth = 8;
            }

            unpackModulations(P, 0, 0, i32ModulationValues, i32ModulationModes, ui8Bpp);
            unpackModulations(Q, (int)ui32WordWidth, 0, i32ModulationValues, i32ModulationModes, ui8Bpp);
            unpackModulations(R, 0, (int)ui32WordHeight, i32ModulationValues, i32ModulationModes, ui8Bpp);
            unpackModulations(S, (int)ui32WordWidth, (int)ui32WordHeight, i32ModulationValues, i32ModulationModes, ui8Bpp);

            interpolateColors(getColorA(P.u32ColorData), getColorA(Q.u32ColorData), getColorA(R.u32ColorData), getColorA(S.u32ColorData), ref upscaledColorA, ui8Bpp);
            interpolateColors(getColorB(P.u32ColorData), getColorB(Q.u32ColorData), getColorB(R.u32ColorData), getColorB(S.u32ColorData), ref upscaledColorB, ui8Bpp);

            for (uint y = 0; y < ui32WordHeight; y++)
            {
                for (uint x = 0; x < ui32WordWidth; x++)
                {
                    int mod = getModulationValues(i32ModulationValues, i32ModulationModes, x + ui32WordWidth / 2, y + ui32WordHeight / 2, ui8Bpp);
                    bool punchthroughAlpha = false;
                    if (mod > 10)
                    {
                        punchthroughAlpha = true;
                        mod -= 10;
                    }

                    Pixel128S result;
                    result.red = (upscaledColorA[y * ui32WordWidth + x].red * (8 - mod) + upscaledColorB[y * ui32WordWidth + x].red * mod) / 8;
                    result.green = (upscaledColorA[y * ui32WordWidth + x].green * (8 - mod) + upscaledColorB[y * ui32WordWidth + x].green * mod) / 8;
                    result.blue = (upscaledColorA[y * ui32WordWidth + x].blue * (8 - mod) + upscaledColorB[y * ui32WordWidth + x].blue * mod) / 8;

                    if (punchthroughAlpha)
                        result.alpha = 0;
                    else
                        result.alpha = (upscaledColorA[y * ui32WordWidth + x].alpha * (8 - mod) + upscaledColorB[y * ui32WordWidth + x].alpha * mod) / 8;

                    if (ui8Bpp == 2)
                    {
                        pColorData[y * ui32WordWidth + x].red = (byte)result.red;
                        pColorData[y * ui32WordWidth + x].green = (byte)result.green;
                        pColorData[y * ui32WordWidth + x].blue = (byte)result.blue;
                        pColorData[y * ui32WordWidth + x].alpha = (byte)result.alpha;
                    }
                    else if (ui8Bpp == 4)
                    {
                        pColorData[y + x * ui32WordHeight].red = (byte)result.red;
                        pColorData[y + x * ui32WordHeight].green = (byte)result.green;
                        pColorData[y + x * ui32WordHeight].blue = (byte)result.blue;
                        pColorData[y + x * ui32WordHeight].alpha = (byte)result.alpha;
                    }
                }
            }
        }

        static uint wrapWordIndex(uint numWords, int word)
        {
            return (uint)((word + numWords) % numWords);
        }

        static bool isPowerOf2(uint input)
        {
            uint minus1;

            if (input == 0) { return false; }

            minus1 = input - 1;
            return ((input | minus1) == (input ^ minus1));
        }

        static uint TwiddleUV(uint XSize, uint YSize, uint XPos, uint YPos)
        {
            uint MinDimension = XSize;
            uint MaxValue = YPos;
            uint Twiddled = 0;
            uint SrcBitPos = 1;
            uint DstBitPos = 1;
            int ShiftCount = 0;

            System.Diagnostics.Debug.Assert(YPos < YSize);
            System.Diagnostics.Debug.Assert(XPos < XSize);
            System.Diagnostics.Debug.Assert(isPowerOf2(YSize));
            System.Diagnostics.Debug.Assert(isPowerOf2(XSize));

            if (YSize < XSize)
            {
                MinDimension = YSize;
                MaxValue = XPos;
            }

            while (SrcBitPos < MinDimension)
            {
                if ((YPos & SrcBitPos) != 0)
                {
                    Twiddled |= DstBitPos;
                }

                if ((XPos & SrcBitPos) != 0)
                {
                    Twiddled |= (DstBitPos << 1);
                }

                SrcBitPos <<= 1;
                DstBitPos <<= 2;
                ShiftCount += 1;
            }

            MaxValue >>= ShiftCount;
            Twiddled |= (MaxValue << (2 * ShiftCount));

            return Twiddled;
        }

        static void mapDecompressedData(ref Pixel32[] pOutput, int width, Pixel32[] pWord, PVRTCWordIndices words, byte ui8Bpp)
        {
            uint ui32WordWidth = 4;
            uint ui32WordHeight = 4;
            if (ui8Bpp == 2)
            {
                ui32WordWidth = 8;
            }

            for (uint y = 0; y < ui32WordHeight / 2; y++)
            {
                for (uint x = 0; x < ui32WordWidth / 2; x++)
                {
                    pOutput[(((words.P[1] * ui32WordHeight) + y + ui32WordHeight / 2)
                             * width + words.P[0] * ui32WordWidth + x + ui32WordWidth / 2)] = pWord[y * ui32WordWidth + x];

                    pOutput[(((words.Q[1] * ui32WordHeight) + y + ui32WordHeight / 2)
                             * width + words.Q[0] * ui32WordWidth + x)] = pWord[y * ui32WordWidth + x + ui32WordWidth / 2];

                    pOutput[(((words.R[1] * ui32WordHeight) + y)
                             * width + words.R[0] * ui32WordWidth + x + ui32WordWidth / 2)] = pWord[(y + ui32WordHeight / 2) * ui32WordWidth + x];

                    pOutput[(((words.S[1] * ui32WordHeight) + y)
                             * width + words.S[0] * ui32WordWidth + x)] = pWord[(y + ui32WordHeight / 2) * ui32WordWidth + x + ui32WordWidth / 2];
                }
            }
        }

        static int pvrtcDecompress(byte[] pCompressedData, ref Pixel32[] pDecompressedData, uint ui32Width, uint ui32Height, byte ui8Bpp)
        {
            uint ui32WordWidth = 4;
            uint ui32WordHeight = 4;
            if (ui8Bpp == 2)
            {
                ui32WordWidth = 8;
            }

            uint[] pWordMembers = new uint[pCompressedData.Length / 4];
            for (int i = 0; i < pCompressedData.Length; i += 4) pWordMembers[i / 4] = BitConverter.ToUInt32(pCompressedData, i);

            int i32NumXWords = (int)(ui32Width / ui32WordWidth);
            int i32NumYWords = (int)(ui32Height / ui32WordHeight);

            PVRTCWordIndices indices;
            Pixel32[] pPixels = new Pixel32[ui32WordWidth * ui32WordHeight];

            for (int wordY = -1; wordY < i32NumYWords - 1; wordY++)
            {
                for (int wordX = -1; wordX < i32NumXWords - 1; wordX++)
                {
                    indices = new PVRTCWordIndices(
                        (int)wrapWordIndex((uint)i32NumXWords, wordX),
                        (int)wrapWordIndex((uint)i32NumYWords, wordY),
                        (int)wrapWordIndex((uint)i32NumXWords, wordX + 1),
                        (int)wrapWordIndex((uint)i32NumYWords, wordY),
                        (int)wrapWordIndex((uint)i32NumXWords, wordX),
                        (int)wrapWordIndex((uint)i32NumYWords, wordY + 1),
                        (int)wrapWordIndex((uint)i32NumXWords, wordX + 1),
                        (int)wrapWordIndex((uint)i32NumYWords, wordY + 1));

                    uint[] WordOffsets = new uint[4]
                    {
                        TwiddleUV((uint)i32NumXWords, (uint)i32NumYWords, (uint)indices.P[0], (uint)indices.P[1]) * 2,
                        TwiddleUV((uint)i32NumXWords, (uint)i32NumYWords, (uint)indices.Q[0], (uint)indices.Q[1]) * 2,
                        TwiddleUV((uint)i32NumXWords, (uint)i32NumYWords, (uint)indices.R[0], (uint)indices.R[1]) * 2,
                        TwiddleUV((uint)i32NumXWords, (uint)i32NumYWords, (uint)indices.S[0], (uint)indices.S[1]) * 2,
                    };

                    PVRTCWord P, Q, R, S;
                    P.u32ColorData = pWordMembers[WordOffsets[0] + 1];
                    P.u32ModulationData = pWordMembers[WordOffsets[0]];
                    Q.u32ColorData = pWordMembers[WordOffsets[1] + 1];
                    Q.u32ModulationData = pWordMembers[WordOffsets[1]];
                    R.u32ColorData = pWordMembers[WordOffsets[2] + 1];
                    R.u32ModulationData = pWordMembers[WordOffsets[2]];
                    S.u32ColorData = pWordMembers[WordOffsets[3] + 1];
                    S.u32ModulationData = pWordMembers[WordOffsets[3]];

                    pvrtcGetDecompressedPixels(P, Q, R, S, ref pPixels, ui8Bpp);
                    mapDecompressedData(ref pDecompressedData, (int)ui32Width, pPixels, indices, ui8Bpp);
                }
            }

            return (int)(ui32Width * ui32Height / (uint)(ui32WordWidth / 2));
        }

        static int PVRTDecompressPVRTC(byte[] pCompressedData, int Do2bitMode, int XDim, int YDim, int IsPVRII, ref byte[] pResultImage)
        {
            Pixel32[] pDecompressedData;

            int XTrueDim = Math.Max(XDim, ((Do2bitMode == 1) ? 16 : 8));
            int YTrueDim = Math.Max(YDim, 8);

            if (XTrueDim != XDim || YTrueDim != YDim)
                pDecompressedData = new Pixel32[XTrueDim * YTrueDim];
            else
                pDecompressedData = new Pixel32[XDim * YDim];

            int retval = pvrtcDecompress(pCompressedData, ref pDecompressedData, (uint)XTrueDim, (uint)YTrueDim, (byte)(Do2bitMode == 1 ? 2 : 4));

            for (int x = 0; x < XDim; ++x)
            {
                for (int y = 0; y < YDim; ++y)
                {
                    pResultImage[(x + y * XDim) * 4 + 2] = pDecompressedData[x + y * XTrueDim].red;
                    pResultImage[(x + y * XDim) * 4 + 1] = pDecompressedData[x + y * XTrueDim].green;
                    pResultImage[(x + y * XDim) * 4 + 0] = pDecompressedData[x + y * XTrueDim].blue;
                    pResultImage[(x + y * XDim) * 4 + 3] = pDecompressedData[x + y * XTrueDim].alpha;
                }
            }
            return retval;
        }
    }

    public static class PostProcessing
    {
        static readonly int[] tileOrder =
        {
            0, 1, 8, 9,
            2, 3, 10, 11,
            16, 17, 24, 25,
            18, 19, 26, 27,

            4, 5, 12, 13,
            6, 7, 14, 15,
            20, 21, 28, 29,
            22, 23, 30, 31,

            32, 33, 40, 41,
            34, 35, 42, 43,
            48, 49, 56, 57,
            50, 51, 58, 59,

            36, 37, 44, 45,
            38, 39, 46, 47,
            52, 53, 60, 61,
            54, 55, 62, 63
        };

        private static int GetTilePixelIndex(int t, int x, int y, int width)
        {
            return (int)((((tileOrder[t] / 8) + y) * width) + ((tileOrder[t] % 8) + x));
        }

        private static int GetTilePixelOffset(int t, int x, int y, int width, int bytesPerPixel)
        {
            return (GetTilePixelIndex(t, x, y, width) * bytesPerPixel);
        }

        public static byte[] UntileTexture(byte[] pixelData, int width, int height, int bytesPerPixel)
        {
            byte[] untiled = new byte[pixelData.Length];

            int s = 0;
            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int t = 0; t < (8 * 8); t++)
                    {
                        int pixelOffset = GetTilePixelOffset(t, x, y, width, bytesPerPixel);
                        Buffer.BlockCopy(pixelData, s, untiled, pixelOffset, bytesPerPixel);
                        s += bytesPerPixel;
                    }
                }
            }

            return untiled;
        }

        private static int Compact1By1(int x)
        {
            x &= 0x55555555;
            x = (x ^ (x >> 1)) & 0x33333333;
            x = (x ^ (x >> 2)) & 0x0f0f0f0f;
            x = (x ^ (x >> 4)) & 0x00ff00ff;
            x = (x ^ (x >> 8)) & 0x0000ffff;
            return x;
        }

        private static int DecodeMorton2X(int code)
        {
            return Compact1By1(code >> 0);
        }

        private static int DecodeMorton2Y(int code)
        {
            return Compact1By1(code >> 1);
        }

        public static byte[] UnswizzleTexture(byte[] pixelData, int width, int height, int bytesPerPixel)
        {
            byte[] unswizzled = new byte[pixelData.Length];

            for (int i = 0; i < width * height; i++)
            {
                int min = width < height ? width : height;
                int k = (int)Math.Log(min, 2);

                int x, y;
                if (height < width)
                {
                    int j = i >> (2 * k) << (2 * k)
                        | (DecodeMorton2Y(i) & (min - 1)) << k
                        | (DecodeMorton2X(i) & (min - 1)) << 0;
                    x = j / height;
                    y = j % height;
                }
                else
                {
                    int j = i >> (2 * k) << (2 * k)
                        | (DecodeMorton2X(i) & (min - 1)) << k
                        | (DecodeMorton2Y(i) & (min - 1)) << 0;
                    x = j % width;
                    y = j / width;
                }

                if (y >= height || x >= width) continue;

                Buffer.BlockCopy(pixelData, i * bytesPerPixel, unswizzled, ((y * width) + x) * bytesPerPixel, bytesPerPixel);
            }

            return unswizzled;
        }
    }

    public static class PixelDataProviders
    {
        public static readonly Dictionary<SceGxmTextureFormat, (int BytesPerPixel, Type? PixelType)> PixelFormatMap =
            new Dictionary<SceGxmTextureFormat, (int BytesPerPixel, Type? PixelType)>()
        {
            { SceGxmTextureFormat.U8_1RRR, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.U8_R000, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.U8_R111, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.U8U8_RGGG, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.U8U8_00GR, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.U1U5U5U5_ARGB, (2, typeof(Rgba32)) },
            { SceGxmTextureFormat.U4U4U4U4_ARGB, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.U5U6U5_RGB, (2, typeof(Rgba32)) },
            { SceGxmTextureFormat.U8U8U8U8_ARGB, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.UBC1_ABGR, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.UBC2_ABGR, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.UBC3_ABGR, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.PVRT2BPP_ABGR, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.PVRT4BPP_ABGR, (4, typeof(Rgba32)) },
            { SceGxmTextureFormat.U8U8U8_RGB, (3, typeof(Rgb24)) },
            { SceGxmTextureFormat.U8U8U8X8_RGB1, (4, typeof(Rgb24)) },
            { SceGxmTextureFormat.P4_ABGR, (0, null) },
            { SceGxmTextureFormat.P4_ARGB, (0, null) },
            { SceGxmTextureFormat.P4_RGBA, (0, null) },
            { SceGxmTextureFormat.P4_BGRA, (0, null) },
            { SceGxmTextureFormat.P4_1BGR, (0, null) },
            { SceGxmTextureFormat.P4_1RGB, (0, null) },
            { SceGxmTextureFormat.P4_RGB1, (0, null) },
            { SceGxmTextureFormat.P4_BGR1, (0, null) },
            { SceGxmTextureFormat.P8_ABGR, (1, typeof(L8)) },
            { SceGxmTextureFormat.P8_ARGB, (1, typeof(L8)) },
            { SceGxmTextureFormat.P8_RGBA, (1, typeof(L8)) },
            { SceGxmTextureFormat.P8_BGRA, (1, typeof(L8)) },
            { SceGxmTextureFormat.P8_1BGR, (1, typeof(L8)) },
            { SceGxmTextureFormat.P8_1RGB, (1, typeof(L8)) },
            { SceGxmTextureFormat.P8_RGB1, (1, typeof(L8)) },
            { SceGxmTextureFormat.P8_BGR1, (1, typeof(L8)) },
        };

        public delegate byte[] ProviderFunctionDelegate(BinaryReader reader, SceGxtTextureInfo info);

        public static readonly Dictionary<SceGxmTextureFormat, ProviderFunctionDelegate> ProviderFunctions =
            new Dictionary<SceGxmTextureFormat, ProviderFunctionDelegate>()
        {
            { SceGxmTextureFormat.U8_1RRR, new ProviderFunctionDelegate(PixelProviderU8_1RRR) },
            { SceGxmTextureFormat.U8_R000, new ProviderFunctionDelegate(PixelProviderU8_R000) },
            { SceGxmTextureFormat.U8_R111, new ProviderFunctionDelegate(PixelProviderU8_R111) },
            { SceGxmTextureFormat.U8U8_RGGG, new ProviderFunctionDelegate(PixelProviderU8U8_RGGG) },
            { SceGxmTextureFormat.U8U8_00GR, new ProviderFunctionDelegate(PixelProviderU8U8_00GR) },
            { SceGxmTextureFormat.U1U5U5U5_ARGB, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.U4U4U4U4_ARGB, new ProviderFunctionDelegate(PixelProviderU4U4U4U4_ARGB) },
            { SceGxmTextureFormat.U5U6U5_RGB, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.U8U8U8U8_ARGB, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.UBC1_ABGR, new ProviderFunctionDelegate(PixelProviderDXTx) },
            { SceGxmTextureFormat.UBC2_ABGR, new ProviderFunctionDelegate(PixelProviderDXTx) },
            { SceGxmTextureFormat.UBC3_ABGR, new ProviderFunctionDelegate(PixelProviderDXTx) },
            { SceGxmTextureFormat.PVRT2BPP_ABGR, new ProviderFunctionDelegate(PixelProviderPVRTC) },
            { SceGxmTextureFormat.PVRT4BPP_ABGR, new ProviderFunctionDelegate(PixelProviderPVRTC) },
            { SceGxmTextureFormat.U8U8U8_RGB, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.U8U8U8X8_RGB1, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P4_ABGR, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P4_ARGB, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P4_RGBA, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P4_BGRA, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P4_1BGR, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P4_1RGB, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P4_RGB1, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P4_BGR1, new ProviderFunctionDelegate(PixelProviderP4) },
            { SceGxmTextureFormat.P8_ABGR, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P8_ARGB, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P8_RGBA, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P8_BGRA, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P8_1BGR, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P8_1RGB, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P8_RGB1, new ProviderFunctionDelegate(PixelProviderDirect) },
            { SceGxmTextureFormat.P8_BGR1, new ProviderFunctionDelegate(PixelProviderDirect) },
        };

        private static byte[] PixelProviderDirect(BinaryReader reader, SceGxtTextureInfo info)
        {
            return reader.ReadBytes((int)info.DataSize);
        }

        private static byte[] PixelProviderDXTx(BinaryReader reader, SceGxtTextureInfo info)
        {
            return DXTx.Decompress(reader, info);
        }

        private static byte[] PixelProviderPVRTC(BinaryReader reader, SceGxtTextureInfo info)
        {
            return PVRTC.Decompress(reader, info);
        }

        private static byte[] PixelProviderP4(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize];
            for (int i = 0; i < pixelData.Length; i++)
            {
                byte idx = reader.ReadByte();
                pixelData[i] = (byte)(idx >> 4 | idx << 4);
            }
            return pixelData;
        }

        private static byte[] PixelProviderU8_1RRR(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 4];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 0] = pixelData[i + 1] = pixelData[i + 2] = reader.ReadByte();
                pixelData[i + 3] = 0xFF;
            }
            return pixelData;
        }

        private static byte[] PixelProviderU8_R000(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 4];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 0] = pixelData[i + 1] = pixelData[i + 2] = 0x00;
                pixelData[i + 3] = reader.ReadByte();
            }
            return pixelData;
        }

        private static byte[] PixelProviderU8_R111(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 4];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 0] = pixelData[i + 1] = pixelData[i + 2] = 0xff;
                pixelData[i + 3] = reader.ReadByte();
            }
            return pixelData;
        }

        private static byte[] PixelProviderU8U8_RGGG(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 4];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 3] = reader.ReadByte();
                pixelData[i + 0] = pixelData[i + 1] = pixelData[i + 2] = reader.ReadByte();
            }
            return pixelData;
        }

        private static byte[] PixelProviderU8U8_00GR(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 4];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 1] = reader.ReadByte();
                pixelData[i + 2] = reader.ReadByte();
                pixelData[i + 0] = pixelData[i + 3] = 0x00;
            }
            return pixelData;
        }

        private static byte[] PixelProviderU4U4U4U4_ARGB(BinaryReader reader, SceGxtTextureInfo info)
        {
            byte[] pixelData = new byte[info.DataSize * 4];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                ushort rgba = reader.ReadUInt16();
                pixelData[i + 0] = (byte)(((rgba >> 4) << 4) & 0xFF);
                pixelData[i + 1] = (byte)(((rgba >> 8) << 4) & 0xFF);
                pixelData[i + 2] = (byte)(((rgba >> 12) << 4) & 0xFF);
                pixelData[i + 3] = (byte)((rgba << 4) & 0xFF);
            }
            return pixelData;
        }
    }

    public class TextureBundle
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int PaletteIndex { get; private set; }
        public int RawLineSize { get; private set; }
        public SceGxmTextureFormat TextureFormat { get; private set; }

        public int BytesPerPixel { get; private set; }
        public Type? PixelType { get; private set; }
        public byte[] PixelData { get; private set; } = Array.Empty<byte>();
        public int RoundedWidth { get; private set; }
        public int RoundedHeight { get; private set; }

        bool isCompressed;

        public TextureBundle(BinaryReader reader, SceGxtHeader header, SceGxtTextureInfo info)
        {
            reader.BaseStream.Seek(info.DataOffset, SeekOrigin.Begin);

            Width = info.GetWidth();
            Height = info.GetHeight();
            PaletteIndex = info.PaletteIndex;
            RawLineSize = (int)(info.DataSize / info.GetHeightRounded());
            TextureFormat = info.GetTextureFormat();

            if (!PixelDataProviders.PixelFormatMap.ContainsKey(TextureFormat) || !PixelDataProviders.ProviderFunctions.ContainsKey(TextureFormat))
                throw new FormatNotImplementedException(TextureFormat);

            var (bytesPerPixel, pixelType) = PixelDataProviders.PixelFormatMap[TextureFormat];
            BytesPerPixel = bytesPerPixel;
            PixelType = pixelType;
            PixelData = PixelDataProviders.ProviderFunctions[TextureFormat](reader, info);

            SceGxmTextureBaseFormat textureBaseFormat = info.GetTextureBaseFormat();

            isCompressed = (textureBaseFormat == SceGxmTextureBaseFormat.UBC1 || textureBaseFormat == SceGxmTextureBaseFormat.UBC2 || textureBaseFormat == SceGxmTextureBaseFormat.UBC3 ||
                textureBaseFormat == SceGxmTextureBaseFormat.PVRT2BPP || textureBaseFormat == SceGxmTextureBaseFormat.PVRT4BPP ||
                textureBaseFormat == SceGxmTextureBaseFormat.PVRTII2BPP || textureBaseFormat == SceGxmTextureBaseFormat.PVRTII4BPP);

            if (isCompressed)
            {
                RoundedWidth = info.GetWidthRounded();
                RoundedHeight = info.GetHeightRounded();
            }
            else
            {
                RoundedWidth = Width;
                RoundedHeight = Height;
            }

            if (textureBaseFormat != SceGxmTextureBaseFormat.PVRT2BPP && textureBaseFormat != SceGxmTextureBaseFormat.PVRT4BPP &&
                textureBaseFormat != SceGxmTextureBaseFormat.PVRTII2BPP && textureBaseFormat != SceGxmTextureBaseFormat.PVRTII4BPP)
            {
                SceGxmTextureType textureType = info.GetTextureType();
                switch (textureType)
                {
                    case SceGxmTextureType.Linear:
                        break;

                    case SceGxmTextureType.Tiled:
                        if (BytesPerPixel > 0)
                            PixelData = PostProcessing.UntileTexture(PixelData, RoundedWidth, RoundedHeight, BytesPerPixel);
                        break;

                    case SceGxmTextureType.Swizzled:
                    case SceGxmTextureType.Cube:
                        if (BytesPerPixel > 0)
                            PixelData = PostProcessing.UnswizzleTexture(PixelData, RoundedWidth, RoundedHeight, BytesPerPixel);
                        break;

                    case (SceGxmTextureType)0xA0000000:
                        if (BytesPerPixel > 0)
                            PixelData = PostProcessing.UnswizzleTexture(PixelData, RoundedWidth, RoundedHeight, BytesPerPixel);
                        break;

                    default:
                        throw new TypeNotImplementedException(textureType);
                }
            }
        }

        public Image CreateTexture(SixLabors.ImageSharp.Color[]? palette = null)
        {
            if (BytesPerPixel == 0 && PixelType == null)
            {
                return CreateIndexedImage(palette);
            }

            if (PixelType == typeof(Rgba32))
            {
                var image = new Image<Rgba32>(RoundedWidth, RoundedHeight);
                CopyPixelDataToImage(image);
                return CropIfNeeded(image);
            }
            else if (PixelType == typeof(Rgb24))
            {
                var image = new Image<Rgb24>(RoundedWidth, RoundedHeight);
                CopyPixelDataToImage(image);
                return CropIfNeeded(image);
            }
            else if (PixelType == typeof(L8))
            {
                var image = new Image<L8>(RoundedWidth, RoundedHeight);
                CopyPixelDataToImage(image);

                if (palette != null && palette.Length > 0)
                {
                    var rgbaImage = image.CloneAs<Rgba32>();
                    ApplyPalette(rgbaImage, palette);
                    image.Dispose();
                    return CropIfNeeded(rgbaImage);
                }
                return CropIfNeeded(image);
            }

            throw new NotSupportedException($"不支持的像素类型:{PixelType}");
        }

        private Image CreateIndexedImage(SixLabors.ImageSharp.Color[]? palette)
        {
            if (palette == null || palette.Length == 0)
                throw new ArgumentNullException(nameof(palette), "索引格式需要调色板");

            var image = new Image<L8>(RoundedWidth, RoundedHeight);
            CopyIndexedDataToImage(image);

            var rgbaImage = image.CloneAs<Rgba32>();
            ApplyPalette(rgbaImage, palette);
            image.Dispose();

            return CropIfNeeded(rgbaImage);
        }

        private void CopyPixelDataToImage<T>(Image<T> image) where T : unmanaged, IPixel<T>
        {
            int pixelIndex = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int dataOffset = pixelIndex * BytesPerPixel;
                    if (dataOffset + BytesPerPixel <= PixelData.Length)
                    {
                        if (typeof(T) == typeof(Rgba32))
                        {
                            var pixel = new Rgba32(
                                PixelData[dataOffset + 2],  // R
                                PixelData[dataOffset + 1],  // G
                                PixelData[dataOffset + 0],  // B
                                PixelData[dataOffset + 3]); // A
                            image[x, y] = (T)(object)pixel;
                        }
                        else if (typeof(T) == typeof(Rgb24))
                        {
                            var pixel = new Rgb24(
                                PixelData[dataOffset + 2],  // R
                                PixelData[dataOffset + 1],  // G
                                PixelData[dataOffset + 0]); // B
                            image[x, y] = (T)(object)pixel;
                        }
                        else if (typeof(T) == typeof(L8))
                        {
                            var pixel = new L8(PixelData[dataOffset]);
                            image[x, y] = (T)(object)pixel;
                        }
                    }
                    pixelIndex++;
                }
            }
        }

        private void CopyIndexedDataToImage(Image<L8> image)
        {
            int pixelIndex = 0;

            if (TextureFormat.ToString().StartsWith("P4"))
            {
                for (int i = 0; i < PixelData.Length && pixelIndex < image.Width * image.Height; i++)
                {
                    byte packed = PixelData[i];
                    int x = pixelIndex % image.Width;
                    int y = pixelIndex / image.Width;

                    image[x, y] = new L8((byte)((packed & 0xF0) >> 4));
                    pixelIndex++;

                    if (pixelIndex < image.Width * image.Height)
                    {
                        x = pixelIndex % image.Width;
                        y = pixelIndex / image.Width;
                        image[x, y] = new L8((byte)(packed & 0x0F));
                        pixelIndex++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < PixelData.Length && pixelIndex < image.Width * image.Height; i++)
                {
                    int x = pixelIndex % image.Width;
                    int y = pixelIndex / image.Width;
                    image[x, y] = new L8(PixelData[i]);
                    pixelIndex++;
                }
            }
        }

        private void ApplyPalette(Image<Rgba32> image, SixLabors.ImageSharp.Color[] palette)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image[x, y];
                    int index = pixel.R;
                    if (index < palette.Length)
                    {
                        var color = palette[index];
                        var pixelColor = color.ToPixel<Rgba32>();
                        image[x, y] = pixelColor;
                    }
                }
            }
        }

        private Image CropIfNeeded<T>(Image<T> image) where T : unmanaged, IPixel<T>
        {
            if (Width != RoundedWidth || Height != RoundedHeight)
            {
                var cropped = image.Clone(ctx => ctx.Crop(Width, Height));
                image.Dispose();
                return cropped;
            }
            return image;
        }
    }

    public class GxtBinary : IDisposable
    {
        public SceGxtHeader Header { get; private set; }
        public SceGxtTextureInfo[] TextureInfos { get; private set; } = Array.Empty<SceGxtTextureInfo>();

        public BUVChunk? BUVChunk { get; private set; }

        public uint[][] P4Palettes { get; private set; } = Array.Empty<uint[]>();
        public uint[][] P8Palettes { get; private set; } = Array.Empty<uint[]>();

        public TextureBundle[] TextureBundles { get; private set; } = Array.Empty<TextureBundle>();

        public Image[] Textures { get; private set; } = Array.Empty<Image>();
        public Image[] BUVTextures { get; private set; } = Array.Empty<Image>();

        public GxtBinary(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            Header = new SceGxtHeader(stream);

            Func<Stream, SceGxtTextureInfo> textureInfoGeneratorFunc;
            switch (Header.Version)
            {
                case 0x10000003: textureInfoGeneratorFunc = new Func<Stream, SceGxtTextureInfo>((s) => { return new SceGxtTextureInfoV301(s); }); break;
                case 0x10000002: textureInfoGeneratorFunc = new Func<Stream, SceGxtTextureInfo>((s) => { return new SceGxtTextureInfoV201(s); }); break;
                case 0x10000001: textureInfoGeneratorFunc = new Func<Stream, SceGxtTextureInfo>((s) => { return new SceGxtTextureInfoV101(s); }); break;
                default: throw new VersionNotImplementedException(Header.Version);
            }

            TextureInfos = new SceGxtTextureInfo[Header.NumTextures];
            for (int i = 0; i < TextureInfos.Length; i++)
                TextureInfos[i] = textureInfoGeneratorFunc(stream);

            long currentPosition = stream.Position;
            byte[] magicBytes = reader.ReadBytes(4);
            string magicString = Encoding.ASCII.GetString(magicBytes);

            if (magicString == BUVChunk.ExpectedMagicNumber)
            {
                stream.Seek(-4, SeekOrigin.Current);
                BUVChunk = new BUVChunk(stream);
            }

            ReadAllBasePalettes(reader);
            ReadAllTextures(reader);

            if (BUVChunk != null && TextureBundles.Length > 0)
            {
                TextureBundle bundle = TextureBundles[0];

                BUVTextures = new Image[BUVChunk.Entries.Length];
                for (int i = 0; i < BUVTextures.Length; i++)
                {
                    BUVEntry entry = BUVChunk.Entries[i];
                    using (Image sourceImage = bundle.CreateTexture(FetchPalette(bundle.TextureFormat, entry.PaletteIndex)))
                    {
                        BUVTextures[i] = sourceImage.Clone(ctx => ctx.Crop(new Rectangle(entry.X, entry.Y, entry.Width, entry.Height)));
                    }
                }
            }
        }

        private void ReadAllBasePalettes(BinaryReader reader)
        {
            long paletteOffset = (Header.TextureDataOffset + Header.TextureDataSize) - (((Header.NumP8Palettes * 256) * 4) + ((Header.NumP4Palettes * 16) * 4));
            reader.BaseStream.Seek(paletteOffset, SeekOrigin.Begin);

            P4Palettes = new uint[Header.NumP4Palettes][];
            for (int i = 0; i < P4Palettes.Length; i++) P4Palettes[i] = ReadBasePalette(reader, 16);

            P8Palettes = new uint[Header.NumP8Palettes][];
            for (int i = 0; i < P8Palettes.Length; i++) P8Palettes[i] = ReadBasePalette(reader, 256);
        }

        private void ReadAllTextures(BinaryReader reader)
        {
            TextureBundles = new TextureBundle[TextureInfos.Length];
            Textures = new Image[TextureBundles.Length];

            for (int i = 0; i < TextureInfos.Length; i++)
            {
                try
                {
                    TextureBundle bundle = (TextureBundles[i] = new TextureBundle(reader, Header, TextureInfos[i]));
                    Textures[i] = bundle.CreateTexture(FetchPalette(bundle.TextureFormat, bundle.PaletteIndex));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"警告:无法加载纹理{i}:{ex.Message}");
                    Textures[i] = new Image<Rgba32>(1, 1);
                }
            }
        }

        private uint[] ReadBasePalette(BinaryReader reader, int numColor)
        {
            uint[] palette = new uint[numColor];
            for (int i = 0; i < palette.Length; i++) palette[i] = reader.ReadUInt32();
            return palette;
        }

        private SixLabors.ImageSharp.Color[]? FetchPalette(SceGxmTextureFormat textureFormat, int paletteIndex)
        {
            if (paletteIndex == -1 || (paletteIndex >= P4Palettes.Length && paletteIndex >= P8Palettes.Length))
                return null;

            SixLabors.ImageSharp.Color[] palette;
            if (textureFormat.ToString().StartsWith("P4"))
            {
                if (paletteIndex >= P4Palettes.Length)
                    return null;

                palette = CreatePalette(P4Palettes[paletteIndex], textureFormat);
            }
            else if (textureFormat.ToString().StartsWith("P8"))
            {
                if (paletteIndex >= P8Palettes.Length)
                    return null;

                palette = CreatePalette(P8Palettes[paletteIndex], textureFormat);
            }
            else
            {
                return null;
            }

            return palette;
        }

        private SixLabors.ImageSharp.Color[] CreatePalette(uint[] inputPalette, SceGxmTextureFormat format)
        {
            SixLabors.ImageSharp.Color[] outputPalette = new SixLabors.ImageSharp.Color[inputPalette.Length];

            for (int i = 0; i < outputPalette.Length; i++)
            {
                uint color = inputPalette[i];
                byte a = (byte)(color >> 24);
                byte b = (byte)(color >> 0);
                byte g = (byte)(color >> 8);
                byte r = (byte)(color >> 16);

                switch (format)
                {
                    case SceGxmTextureFormat.P4_ABGR:
                    case SceGxmTextureFormat.P8_ABGR:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgba(r, g, b, a);
                        break;

                    case SceGxmTextureFormat.P4_ARGB:
                    case SceGxmTextureFormat.P8_ARGB:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgba(b, g, r, a);
                        break;

                    case SceGxmTextureFormat.P4_RGBA:
                    case SceGxmTextureFormat.P8_RGBA:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgba(a, b, g, r);
                        break;

                    case SceGxmTextureFormat.P4_BGRA:
                    case SceGxmTextureFormat.P8_BGRA:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgba(a, r, g, b);
                        break;

                    case SceGxmTextureFormat.P4_1BGR:
                    case SceGxmTextureFormat.P8_1BGR:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgb(b, g, r);
                        break;

                    case SceGxmTextureFormat.P4_1RGB:
                    case SceGxmTextureFormat.P8_1RGB:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgb(r, g, b);
                        break;

                    case SceGxmTextureFormat.P4_RGB1:
                    case SceGxmTextureFormat.P8_RGB1:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgba(r, g, b, 255);
                        break;

                    case SceGxmTextureFormat.P4_BGR1:
                    case SceGxmTextureFormat.P8_BGR1:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgba(b, g, r, 255);
                        break;

                    default:
                        outputPalette[i] = SixLabors.ImageSharp.Color.FromRgba(r, g, b, a);
                        break;
                }
            }

            return outputPalette;
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Textures != null)
                    {
                        foreach (var texture in Textures)
                        {
                            texture?.Dispose();
                        }
                    }

                    if (BUVTextures != null)
                    {
                        foreach (var texture in BUVTextures)
                        {
                            texture?.Dispose();
                        }
                    }
                }

                disposed = true;
            }
        }

        ~GxtBinary()
        {
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GXT转换器 - PSP/Vita GXT纹理转换工具");
            Console.WriteLine("================================================");

            if (args.Length < 1)
            {
                Console.WriteLine("使用方法:GXTConvert.exe <输入文件.gxt> [输出目录]");
                Console.WriteLine("示例:GXTConvert.exe texture.gxt ./output");
                return;
            }

            string inputFile = args[0];
            string outputDir = args.Length > 1 ? args[1] : Path.GetDirectoryName(inputFile) ?? "./";

            try
            {
                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"错误:输入文件'{inputFile}'不存在。");
                    return;
                }

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                    Console.WriteLine($"已创建输出目录:{outputDir}");
                }

                Console.WriteLine($"正在处理文件:{inputFile}");
                Console.WriteLine($"输出目录:{outputDir}");

                using (FileStream fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (GxtBinary gxt = new GxtBinary(fs))
                {
                    Console.WriteLine($"文件格式:GXT v{((gxt.Header.Version & 0xFF) == 0x03 ? "3.01" : "2.01")}");
                    Console.WriteLine($"纹理数量:{gxt.Textures.Length}");
                    Console.WriteLine($"4位调色板:{gxt.P4Palettes.Length}");
                    Console.WriteLine($"8位调色板:{gxt.P8Palettes.Length}");

                    if (gxt.BUVChunk != null)
                    {
                        Console.WriteLine($"BUV条目:{gxt.BUVChunk.Entries.Length}");
                    }

                    Console.WriteLine("\n正在保存纹理...");
                    for (int i = 0; i < gxt.Textures.Length; i++)
                    {
                        string outputPath = Path.Combine(outputDir, $"texture_{i:000}.png");
                        gxt.Textures[i].Save(outputPath, new PngEncoder());
                        Console.WriteLine($"已保存: {outputPath}");
                    }

                    if (gxt.BUVTextures != null && gxt.BUVTextures.Length > 0)
                    {
                        Console.WriteLine("\n正在保存BUV纹理...");
                        for (int i = 0; i < gxt.BUVTextures.Length; i++)
                        {
                            string outputPath = Path.Combine(outputDir, $"buv_{i:000}.png");
                            gxt.BUVTextures[i].Save(outputPath, new PngEncoder());
                            Console.WriteLine($"已保存BUV纹理:{outputPath}");
                        }
                    }

                    Console.WriteLine("\n================================================");
                    Console.WriteLine("转换完成!");
                    Console.WriteLine($"总计:{gxt.Textures.Length}个主纹理");
                    if (gxt.BUVTextures != null)
                    {
                        Console.WriteLine($"{gxt.BUVTextures.Length}个BUV纹理");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"错误:无法找到文件'{inputFile}'");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"错误:无法访问目录'{outputDir}'");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"错误:没有权限访问文件或目录");
            }
            catch (FormatNotImplementedException ex)
            {
                Console.WriteLine($"错误:不支持的纹理格式:{ex.Format}");
            }
            catch (TypeNotImplementedException ex)
            {
                Console.WriteLine($"错误:不支持的纹理类型:{ex.Type}");
            }
            catch (VersionNotImplementedException ex)
            {
                Console.WriteLine($"错误:不支持的GXT版本:0x{ex.Version:X8}");
            }
            catch (UnknownMagicException ex)
            {
                Console.WriteLine($"错误:未知的文件格式或损坏的文件:{ex.Message}");
            }
            catch (PaletteNotImplementedException ex)
            {
                Console.WriteLine($"错误:不支持的调色板格式:{ex.Format}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误:{ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"内部错误:{ex.InnerException.Message}");
                }
                Console.WriteLine($"\n堆栈跟踪:\n{ex.StackTrace}");
            }
        }
    }
}