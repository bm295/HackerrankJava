private static int B;
private static int H;
private static boolean flag = validateInputs();
private static boolean validateInputs() {
    Scanner scanner = new Scanner(System.in);
    B = scanner.nextInt();
    H = scanner.nextInt();
    if (B > 0 && H > 0) {
        return true;
    }
    else {
        System.out.println("java.lang.Exception: Breadth and height must be positive");
        return false;
    }    
}
