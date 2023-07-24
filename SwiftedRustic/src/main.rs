use std::env;
use windows::{
    core::*, Data::Xml::Dom::*, Win32::Foundation::*, Win32::System::Threading::*,
    Win32::UI::WindowsAndMessaging::*,
};

pub mod swifto {
	pub fn swift(delay: i16) -> i16 {
		println!("{}", delay);

        return delay
	}
}

use crate::swifto::swift;

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn swift_pass_in_number() {
        assert_eq!(5, swift(5));
    }
}

fn parse(args: Vec<String>) -> i16 {
    if args.len() == 1 {
        return 5
    }

    for arg in args.iter().skip(1) {
        return arg.parse().expect("Not a number!");
    }

    return 5
}

fn main() {
    let delay_seconds: i16 = parse(env::args().collect());

    swift(delay_seconds);
}
