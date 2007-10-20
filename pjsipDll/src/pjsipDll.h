// pjsipDll.h : Declares the entry point for the .Net GUI application.
//


#ifdef PJSIPDLL_EXPORTS
	#define PJSIPDLL_DLL_API __declspec(dllexport)
#else
	#define PJSIPDLL_DLL_API __declspec(dllimport)
#endif

// calback function definitions
typedef int __stdcall fptr_regstate(int, int);				// on registration state changed
typedef int __stdcall fptr_callstate(int, int);	// on call state changed
typedef int __stdcall fptr_callincoming(int, char*);	// on call incoming
typedef int __stdcall fptr_getconfigdata(int);	// get config data
typedef int __stdcall fptr_callholdconf(int);
typedef int __stdcall fptr_callretrieveconf(int);
typedef int __stdcall fptr_msgrec (char*, char*);
typedef int __stdcall fptr_buddystatus(int, int);

// Callback registration 
extern "C" PJSIPDLL_DLL_API int onRegStateCallback(fptr_regstate cb);	  // register registration notifier
extern "C" PJSIPDLL_DLL_API int onCallStateCallback(fptr_callstate cb); // register call notifier
extern "C" PJSIPDLL_DLL_API int onCallIncoming(fptr_callincoming cb); // register incoming call notifier
extern "C" PJSIPDLL_DLL_API int getConfigDataCallback(fptr_getconfigdata cb); // get config data
extern "C" PJSIPDLL_DLL_API int onCallHoldConfirmCallback(fptr_callholdconf cb); // register call notifier
//extern "C" PJSIPDLL_DLL_API int onCallRetrieveConfirm(fptr_callretrieveconf cb); // register call notifier
extern "C" PJSIPDLL_DLL_API int onMessageReceivedCallback(fptr_msgrec cb); // register call notifier
extern "C" PJSIPDLL_DLL_API int onBuddyStatusChangedCallback(fptr_buddystatus cb); // register call notifier

// pjsip common API
extern "C" PJSIPDLL_DLL_API int dll_init(int listenPort);
extern "C" PJSIPDLL_DLL_API int dll_shutdown(); 
extern "C" PJSIPDLL_DLL_API int dll_main(void);
// pjsip call API
extern "C" PJSIPDLL_DLL_API int dll_registerAccount(char* uri, char* reguri, char* name, char* username, char* password);
extern "C" PJSIPDLL_DLL_API int dll_makeCall(int accountId, char* uri); 
extern "C" PJSIPDLL_DLL_API int dll_releaseCall(int callId); 
extern "C" PJSIPDLL_DLL_API int dll_answerCall(int callId, int code);
extern "C" PJSIPDLL_DLL_API int dll_holdCall(int callId);
extern "C" PJSIPDLL_DLL_API int dll_retrieveCall(int callId);
extern "C" PJSIPDLL_DLL_API int dll_xferCall(int callid, char* uri);
extern "C" PJSIPDLL_DLL_API int dll_xferCallWithReplaces(int callId, int dstSession);
extern "C" PJSIPDLL_DLL_API int dll_serviceReq(int callId, int serviceCode, const char* destUri);
extern "C" PJSIPDLL_DLL_API int dll_dialDtmf(int callId, char* digits, int mode);
extern "C" PJSIPDLL_DLL_API int dll_removeAccounts();
// IM & Presence api
extern "C" PJSIPDLL_DLL_API int dll_addBuddy(char* uri, bool subscribe);
extern "C" PJSIPDLL_DLL_API int dll_removeBuddy(int buddyId);
extern "C" PJSIPDLL_DLL_API int dll_sendMessage(int accId, char* uri, char* message);
extern "C" PJSIPDLL_DLL_API int dll_setStatus(int accId, int presence_state);
