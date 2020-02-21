#include <Windows.h>

#include <Shellapi.h>

#pragma comment (lib, "Shell32")

int main()
{
	while (true) {
		ShellExecute(NULL, L"open", L"http://www.taylorswift.com", NULL, NULL, SW_SHOWMAXIMIZED);

		Sleep(60000);
	}
}