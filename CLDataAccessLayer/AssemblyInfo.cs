using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

//
// �@�몺�ե��T�O�ѤU�C�o���ݩʩұ���C
// �ܧ�o���ݩʪ��ȧY�i�ק�ե󪺬�����T�C
//
[assembly: AssemblyTitle("��Ƴs���h")]
[assembly: AssemblyDescription("��Ʈw�s������P�ǿ餸��")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// �ե󪺪�����T�ѤU�C�|�ӭȩҲզ�:
//
//      �D�n����
//      ���n���� 
//      �իؽs��
//      �׭q
//
// �z�i�H�ۦ���w�Ҧ����ȡA�]�i�H�̷ӥH�U���覡�A�ϥ� '*' �N�׭q�M�իؽs��
// ���w���w�]��:

[assembly: AssemblyVersion("1.3.*")]

//
// �Y�nñ�W�ե�A�z�������w�ҭn�ϥΪ����_�C
// �p�ݲե�ñ�W���ԲӸ�T�A�аѾ\ Microsoft .NET Framework ���C
//
// �ШϥΤU�C�ݩʨӱ�����Ӫ��_�n�Ψ�ñ�W�C
//
// �`�N: 
//   (*) �p�G�S�����w���_�A�N���|ñ�W�ե�C
//   (*) KeyName �O���w�w�˦b�q���W�K�X�sĶ�A�ȴ��Ѫ� (CSP) �������_�C
//       KeyFile �O���t�����_���ɮסC
//   (*) �p�G�P�ɫ��w�F KeyFile �M KeyName ���ȡA�N�|�o�ͤU�C���B�z�{��:
//       (1) �p�G�b CSP ���i�H��� KeyName�A�N�|�ϥΦ����_�C
//       (2) �p�G KeyName ���s�b�� KeyFile �o�s�b�A�N�|�N KeyFile �������_
//           �w�˨� CSP ���å[�H�ϥΡC
//   (*) �Y�n�إ� KeyFile�A�z�i�H�ϥ� sn.exe (�j���W��) ���ε{���C
//       ���w KeyFile �ɡAKeyFile ����m���Ӭ۹��M�׿�X�ؿ��A��Y
//        %Project Directory%\obj\<configuration>�C
//       �Ҧp�A�p�G�z�� KeyFile �O���M�ץؿ����A�K���ӱN AssemblyKeyFile �ݩʫ��w��
//        [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) ����ñ�W�O�@�Ӷi���ﶵ - �p�ݸԲӸ�T�A�аѾ\ Microsoft .NET Framework ���C
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
[assembly: AssemblyFileVersionAttribute("1.2")]
[assembly: GuidAttribute("E2AC021A-2261-416b-958C-2AE75DE73F28")]
