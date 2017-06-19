#include <iostream>
#include <atlbase.h>
#include <string.h>

#include <objidl.h>
#include <gdiplus.h>

#include <iostream>
#include <boost/asio.hpp>
#include <boost/array.hpp>

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

	HGLOBAL hMem = ::GlobalAlloc(GMEM_MOVEABLE, 1000000);
	LPVOID pImage = ::GlobalLock(hMem);
		::GlobalUnlock(hMem);
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
		char * buf = (char *)malloc(100);
		spStream->Seek({ 0,0 }, STREAM_SEEK_SET, NULL);
		ULONG bytesRead = 0;
		spStream->Read(buf, 100, &bytesRead);
		printf("Bytes Read: %i\n", bytesRead);
		printf(buf);

		delete image;
	}

	GdiplusShutdown(gdiplusToken);
	ReleaseDC(hDesktopWnd, hDesktopDC);
	DeleteDC(hCaptureDC);
	DeleteObject(hCaptureBitmap);
}

using boost::asio::ip::tcp;

boost::asio::io_service io_service;

void BeginServer() {
	boost::asio::io_service io_service;
	tcp::acceptor acceptor(io_service, tcp::endpoint(tcp::v4(), 54321));
	printf("Began listening\n");

	tcp::socket socket(io_service);
	acceptor.accept(socket);

	printf("Connected");
	boost::array<char, 128> buf;
	boost::system::error_code error;

	size_t len = socket.write_some(boost::asio::buffer(buf), error);
	std::cout.write(buf.data(), len);

	CaptureScreen();
}

int main() {
	CaptureScreen();
}