#include <iostream>
#include <atlbase.h>
#include <string.h>

#include <windows.h>
#include <objidl.h>
#include <gdiplus.h>

#include <fstream>
#include <iostream>

using namespace Gdiplus;
#pragma comment (lib, "Gdiplus.lib")

int GetEncoderClsid(const WCHAR* format, CLSID* pClsid)
{
	UINT  num = 0;
	UINT  size = 0;
	ImageCodecInfo* pImageCodecInfo = NULL;

	GetImageEncodersSize(&num, &size);
	if (size == 0)
		return -1;

	pImageCodecInfo = (ImageCodecInfo*)(malloc(size));
	if (pImageCodecInfo == NULL)
		return -1;

	GetImageEncoders(num, size, pImageCodecInfo);

	for (UINT j = 0; j < num; ++j)
	{
		if (wcscmp(pImageCodecInfo[j].MimeType, format) == 0)
		{
			*pClsid = pImageCodecInfo[j].Clsid;
			free(pImageCodecInfo);
			return j;
		}
	}
	free(pImageCodecInfo);
	return -1;
}

void CaptureScreen()
{
	GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR gdiplusToken;
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	HGLOBAL hMem = ::GlobalAlloc(GMEM_FIXED, 1000000);
	CComPtr<IStream> spStream;
	HRESULT hr = ::CreateStreamOnHGlobal(hMem, FALSE, &spStream);

	int nScreenWidth = GetSystemMetrics(SM_CXSCREEN);
	int nScreenHeight = GetSystemMetrics(SM_CYSCREEN);
	HWND hDesktopWnd = GetDesktopWindow();
	HDC hDesktopDC = GetDC(hDesktopWnd);
	HDC hCaptureDC = CreateCompatibleDC(hDesktopDC);
	HBITMAP hCaptureBitmap = CreateCompatibleBitmap(hDesktopDC,
		nScreenWidth, nScreenHeight);

	while (getchar() != 's') {
		SelectObject(hCaptureDC, hCaptureBitmap);
		BitBlt(hCaptureDC, 0, 0, nScreenWidth, nScreenHeight,
			hDesktopDC, 0, 0, SRCCOPY | CAPTUREBLT);

		Bitmap *image = new Bitmap(hCaptureBitmap, NULL);
		CLSID myClsId;
		int retVal = GetEncoderClsid(L"image/png", &myClsId);

		image->Save(spStream, &myClsId, NULL);

		ULARGE_INTEGER pEnd;
		spStream->Seek({ 0,0 }, STREAM_SEEK_CUR, &pEnd);
		printf("Bytes: ");
		std::cout << pEnd.QuadPart << std::endl;

		std::ofstream fout;
		fout.open("file.png", std::ios::binary | std::ios::out);
		fout.write((const char*)hMem, pEnd.QuadPart);

	/*	char * buf = (char *)malloc(100);
		spStream->Seek({ 0,0 }, STREAM_SEEK_SET, NULL);
		ULONG bytesRead = 0;
		spStream->Read(buf, 100, &bytesRead);
		printf("Bytes Read: %i\n", bytesRead);
		printf(buf);
		*/
		delete image;
	}

	GdiplusShutdown(gdiplusToken);
	ReleaseDC(hDesktopWnd, hDesktopDC);
	DeleteDC(hCaptureDC);
	DeleteObject(hCaptureBitmap);
}

int main() {
	CaptureScreen();
}